using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Components;
using WorldTweaker.Utilities;
using static stuffSpawnOnTerrain;

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
			var worldType = WorldTweaker.I.WorldType.Value;
			var terrain = Traverse.Create(__instance).Field("currentPlacedTerrain").GetValue<terrainscript>();
			if (terrain == null) return;

			// Flat world, replace terrain with flat mesh.
			if (worldType == 0)
			{
				var mesh = terrain.meshfilter.mesh;
				var verts = mesh.vertices;
				for (int i = 0; i < verts.Length; i++)
					verts[i].y = 0f;
				mesh.vertices = verts;
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				terrain.meshcollider.sharedMesh = mesh;
			}

			// Liquid tile generation.
			// Distant terrain is enabled, let that handle liquid tiles.
			if (__instance.manager.settings.GenerateDistantTerrain || __instance.manager.settings.GenerateDistantTerrain2)
				return;

			var liquidType = WaterManager.LiquidType.Water;
			switch (worldType)
			{
				// Lava.
				case 3f:
					liquidType = WaterManager.LiquidType.Lava;
					break;
			}

			WorldTweaker.Water.GenerateLiquidTile(liquidType, WaterManager.TerrainType.Close, terrain);
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;
			var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain").GetValue<terrainscript>();
			if (terrain == null) return;

			// Flat world, replace terrain with flat mesh.
			if (worldType == 0)
			{

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

			// Liquid tile generation.
			// Distant terrain 2 is enabled, let that handle liquid tiles.
			if (__instance.manager.settings.GenerateDistantTerrain2)
				return;

			var liquidType = WaterManager.LiquidType.Water;
			switch (worldType)
			{
				// Lava.
				case 3f:
					liquidType = WaterManager.LiquidType.Lava;
					break;
			}

			WorldTweaker.Water.GenerateLiquidTile(liquidType, WaterManager.TerrainType.Distant, terrain);
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain2))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain2
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;
			var terrain = Traverse.Create(__instance).Field("currentPlacedDistantTerrain2").GetValue<terrainscript>();
			if (terrain == null) return;

			// Flat world, replace terrain with flat mesh.
			if (worldType != 0)
			{
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

			// Liquid tile generation.
			var liquidType = WaterManager.LiquidType.Water;
			switch (worldType)
			{
				// Lava.
				case 3f:
					liquidType = WaterManager.LiquidType.Lava;
					break;
			}

			WorldTweaker.Water.GenerateLiquidTile(liquidType, WaterManager.TerrainType.Distant2, terrain);
		}
	}

	[HarmonyPatch(typeof(TerrainManager), "RemoveTerrains")]
	public static class Patch_TerrainManager_RemoveTerrains
	{
		public static void Postfix(TerrainManager __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;

			if (worldType == 2f || worldType == 3f)
			{
				WorldTweaker.Water.RemoveTiles(WaterManager.TerrainType.Close, __instance);
			}
		}
	}

	[HarmonyPatch(typeof(TerrainManager), "RemoveDistantTerrains")]
	public static class Patch_TerrainManager_RemoveDistantTerrains
	{
		public static void Postfix(TerrainManager __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;

			if (worldType == 2f || worldType == 3f)
			{
				WorldTweaker.Water.RemoveTiles(WaterManager.TerrainType.Distant, __instance);
			}
		}
	}

	[HarmonyPatch(typeof(TerrainManager), "RemoveDistantTerrains2")]
	public static class Patch_TerrainManager_RemoveDistantTerrains2
	{
		public static void Postfix(TerrainManager __instance)
		{
			var worldType = WorldTweaker.I.WorldType.Value;

			if (worldType == 2f || worldType == 3f)
			{
				WorldTweaker.Water.RemoveTiles(WaterManager.TerrainType.Distant2, __instance);
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
