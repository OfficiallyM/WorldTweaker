using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Components;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.GetTerrainHeight),
	new Type[] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
	public static class GetTerrainHeight_Postfix
	{
		public static void Postfix(ref float __result)
		{
			float worldType = WorldTweaker.I.WorldType.Value;

			// Make no alterations for vanilla.
			if (worldType == 1)
				return;

			// Force flat.
			if (worldType == 0)
			{
				__result = 0;
				return;
			}
			
			// Road bridge.
			if (worldType == 1.25f)
			{
				__result = 1000f;
				return;
			}

			// Tropical, lava.
			if (worldType == 2f || worldType == 3f)
				return;

			__result *= worldType;
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.GetTerrainHeightWorld),
	new Type[] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
	public static class GetTerrainHeightWorld_Postfix
	{
		public static void Postfix(ref float __result)
		{
			float worldType = WorldTweaker.I.WorldType.Value;

			// Tropical, lava.
			if (worldType == 2f || worldType == 3f)
			{
				__result = Mathf.Clamp(__result, WorldTweaker.Water.WaterHeight + 1f, float.PositiveInfinity);
			}
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.FStart))]
	public static class Patch_TerrainGenerator_FStart
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			WorldTweaker.Water.WaterParent = new GameObject("G_WaterParent").transform;
			WorldTweaker.Water.WaterParent.position = Vector3.zero;
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceTerrain))]
	public static class Patch_TerrainGenerator_PlaceTerrain
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			// Flat world, replace terrain with flat mesh.
			if (WorldTweaker.I.WorldType.Value == 0)
			{
				var terrain = Traverse.Create(__instance).Field("currentPlacedTerrain").GetValue<terrainscript>();
				if (terrain == null) return;

				var mesh = terrain.meshfilter.mesh;
				var verts = mesh.vertices;
				for (int i = 0; i < verts.Length; i++)
					verts[i].y = 0f;
				mesh.vertices = verts;
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				terrain.meshcollider.sharedMesh = mesh;
			}
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;
			// Flat world, replace terrain with flat mesh.
			if (worldType == 0)
			{
				var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain").GetValue<terrainscript>();
				if (terrain == null) return;

				var mesh = terrain.meshfilter.mesh;
				var verts = mesh.vertices;
				for (int i = 0; i < verts.Length; i++)
					verts[i].y = 0f;
				mesh.vertices = verts;
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				if (terrain.meshcollider != null)
					terrain.meshcollider.sharedMesh = mesh;
			}

			// Tropical, place water.
			// Deliberately not using close terrain for better coverage
			// and to avoid overlap.
			if (worldType == 2f)
			{
				var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain").GetValue<terrainscript>();
				if (terrain == null) return;

				if (WorldTweaker.Water.WaterTiles.ContainsKey(terrain)) return;

				var waterPos = new Vector3(terrain.transform.position.x, (float)mainscript.M.mainWorld.coord.y + WorldTweaker.Water.WaterHeight, terrain.transform.position.z);
				var water = GameObject.Instantiate(WorldTweaker.Prefabs.Water, waterPos, Quaternion.identity);
				water.transform.SetParent(WorldTweaker.Water.WaterParent);
				var worldOffset = new Vector3(
					(float)mainscript.M.mainWorld.coord.x,
					0,
					(float)mainscript.M.mainWorld.coord.z
				);
				var size = (TerrainGenerationSettings.staticReference.defDistantTerrainSize / 2f) - 278f;
				var waterMesh = WorldTweaker.Water.GenerateWaterMesh(
					size,
					mainscript.M.holes,
					waterPos - worldOffset,
					water
				);
				water.GetComponent<MeshFilter>().mesh = waterMesh;
				var waterController = water.GetComponent<Water>();
				waterController.SetTextureScale(size);
				WorldTweaker.Water.WaterTiles.Add(terrain, waterController);
			}

			// Lava.
			if (worldType == 3f)
			{
				var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain").GetValue<terrainscript>();
				if (terrain == null) return;

				if (WorldTweaker.Water.LavaTiles.ContainsKey(terrain)) return;

				var lavaPos = new Vector3(terrain.transform.position.x, (float)mainscript.M.mainWorld.coord.y + WorldTweaker.Water.WaterHeight, terrain.transform.position.z);
				var lava = GameObject.Instantiate(WorldTweaker.Prefabs.Lava, lavaPos, Quaternion.identity);
				lava.transform.SetParent(WorldTweaker.Water.WaterParent);
				var worldOffset = new Vector3(
					(float)mainscript.M.mainWorld.coord.x,
					0,
					(float)mainscript.M.mainWorld.coord.z
				);
				var size = (TerrainGenerationSettings.staticReference.defDistantTerrainSize / 2f) - 278f;
				var lavaMesh = WorldTweaker.Water.GenerateWaterMesh(
					size,
					mainscript.M.holes,
					lavaPos - worldOffset,
					lava
				);
				lava.GetComponent<MeshFilter>().mesh = lavaMesh;
				var lavaController = lava.GetComponent<Lava>();
				WorldTweaker.Water.LavaTiles.Add(terrain, lavaController);
			}
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain2))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain2
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			// Flat world, replace terrain with flat mesh.
			if (WorldTweaker.I.WorldType.Value != 0)
			{ 
				var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain2").GetValue<terrainscript>();
				if (terrain == null) return;

				var mesh = terrain.meshfilter.mesh;
				var verts = mesh.vertices;
				for (int i = 0; i < verts.Length; i++)
					verts[i].y = 0f;
				mesh.vertices = verts;
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				if (terrain.meshcollider != null)
					terrain.meshcollider.sharedMesh = mesh;
			}
		}
	}

	[HarmonyPatch(typeof(TerrainManager), "RemoveDistantTerrains")]
	public static class Patch_TerrainManager_RemoveDistantTerrains
	{
		public static void Postfix(TerrainManager __instance)
		{
			var removeQueue = new List<terrainscript>();

			if (WorldTweaker.I.WorldType.Value == 2f)
			{
				foreach (var water in WorldTweaker.Water.WaterTiles)
				{
					if (water.Key == null || !__instance.generator.allterrainsDistant.Contains(water.Key))
						removeQueue.Add(water.Key);
				}
				foreach (var terrain in removeQueue)
				{
					GameObject.Destroy(WorldTweaker.Water.WaterTiles[terrain].gameObject);
					WorldTweaker.Water.WaterTiles.Remove(terrain);
				}
			}

			if (WorldTweaker.I.WorldType.Value == 3f)
			{
				foreach (var lava in WorldTweaker.Water.LavaTiles)
				{
					if (lava.Key == null || !__instance.generator.allterrainsDistant.Contains(lava.Key))
						removeQueue.Add(lava.Key);
				}
				foreach (var terrain in removeQueue)
				{
					GameObject.Destroy(WorldTweaker.Water.LavaTiles[terrain].gameObject);
					WorldTweaker.Water.LavaTiles.Remove(terrain);
				}
			}
		}
	}

	[HarmonyPatch(typeof(terrainHeightAlignToBuildingScript), nameof(terrainHeightAlignToBuildingScript.FStart))]
	internal static class Patch_terrainHeightAlignToBuildingScript_FStart
	{
		private static void Postfix(terrainHeightAlignToBuildingScript __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;
			if (worldType != 2f && worldType != 3f)
				return;

			string name = __instance.name.ToLowerInvariant();
			if (name.Contains("road")) return;

			float multiplier = 3f;
			if (name == "haz02")
				multiplier = 4f;
			for (int i = 0; i < __instance.helpPosList.Count; i++)
			{
				var helpPos = __instance.helpPosList[i];
				helpPos.range = new Vector2(helpPos.range.x * multiplier, helpPos.range.y * multiplier);
			}
		}
	}
}
