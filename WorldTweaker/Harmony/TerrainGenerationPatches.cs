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
			float flatness = WorldTweaker.I.WorldType.Value;

			// Make no alterations for vanilla.
			if (flatness == 1)
				return;

			// Force flat.
			if (flatness == 0)
			{
				__result = 0;
				return;
			}
			
			// Road bridge.
			if (flatness == 1.25f)
			{
				__result = 1000f;
				return;
			}

			// Tropical.
			if (flatness == 2f)
				return;

			__result *= flatness;
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.GetTerrainHeightWorld),
	new Type[] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
	public static class GetTerrainHeightWorld_Postfix
	{
		public static void Postfix(ref float __result)
		{
			float flatness = WorldTweaker.I.WorldType.Value;

			// Tropical.
			if (flatness == 2f)
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
			// Flat world, replace terrain with flat mesh.
			if (WorldTweaker.I.WorldType.Value == 0)
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
			if (WorldTweaker.I.WorldType.Value == 2f)
			{
				var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain").GetValue<terrainscript>();
				if (terrain == null) return;

				if (WorldTweaker.Water.DistantWater.ContainsKey(terrain)) return;

				var waterPos = new Vector3(terrain.transform.position.x, (float)mainscript.M.mainWorld.coord.y + WorldTweaker.Water.WaterHeight, terrain.transform.position.z);
				var water = GameObject.Instantiate(WorldTweaker.Prefabs.Water, waterPos, Quaternion.identity);
				water.transform.SetParent(WorldTweaker.Water.WaterParent);
				var waterController = water.GetComponent<Water>();
				waterController.SetScale((TerrainGenerationSettings.staticReference.defDistantTerrainSize / 2f) - 278f);
				WorldTweaker.Water.DistantWater.Add(terrain, waterController);
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
			foreach (var water in WorldTweaker.Water.DistantWater)
			{
				if (water.Key == null || !__instance.generator.allterrainsDistant.Contains(water.Key))
					removeQueue.Add(water.Key);
			}

			foreach (var terrain in removeQueue)
			{
				GameObject.Destroy(WorldTweaker.Water.DistantWater[terrain].gameObject);
				WorldTweaker.Water.DistantWater.Remove(terrain);
			}
		}
	}

	[HarmonyPatch(typeof(terrainHeightAlignToBuildingScript), nameof(terrainHeightAlignToBuildingScript.FStart))]
	internal static class Patch_terrainHeightAlignToBuildingScript_FStart
	{
		private static void Postfix(terrainHeightAlignToBuildingScript __instance)
		{
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
