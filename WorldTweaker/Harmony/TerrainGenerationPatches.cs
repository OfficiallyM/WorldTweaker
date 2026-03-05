using HarmonyLib;
using System;
using UnityEngine;
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
			if (flatness > 1)
			{
				__result = 1000f;
				return;
			}

			__result *= flatness;
		}
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceTerrain))]
	public static class Patch_TerrainGenerator_PlaceTerrain
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			if (WorldTweaker.I.WorldType.Value != 0) return;

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

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			if (WorldTweaker.I.WorldType.Value != 0) return;

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
	}

	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.PlaceDistantTerrain2))]
	public static class Patch_TerrainGenerator_PlaceDistantTerrain2
	{
		public static void Postfix(TerrainGenerator __instance)
		{
			if (WorldTweaker.I.WorldType.Value != 0) return;

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
