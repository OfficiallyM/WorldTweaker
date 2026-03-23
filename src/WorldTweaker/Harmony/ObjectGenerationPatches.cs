using HarmonyLib;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(ObjectGenerationScript), nameof(ObjectGenerationScript.FStart))]
	internal static class Patch_ObjectGenerationScript_FStart
	{
		private static void Prefix(ObjectGenerationScript __instance)
		{
			foreach (var obj in __instance.objTypes)
			{
				switch (__instance.gameObject.name)
				{
					case "G_GenerationMountains":
						obj.chance *= WorldTweaker.I.MountainDensity.Value;
						break;
					case "G_GenerationDesertTowers":
					case "G_GenerationDesertBuildings":
						obj.chance *= WorldTweaker.I.BuildingDensity.Value;
						break;
					default:
						obj.chance *= WorldTweaker.I.ObjectDensity.Value;
						break;
				}
			}
		}
	}

	[HarmonyPatch(typeof(RandomSpawnedObjScript), nameof(RandomSpawnedObjScript.FStart))]
	internal static class Patch_RandomSpawnedObjScript_FStart
	{
		private static void Postfix(RandomSpawnedObjScript __instance)
		{
			if (WorldTweaker.I.WorldType.Value != 2f) return;

			// Allow mountains to spawn underwater.
			string parent = __instance.transform.parent?.name;
			string name = __instance.name.ToLowerInvariant();
			if (parent == "G_MountainParent")
				return;

			// Allow ships to spawn underwater.
			if (parent == "G_DesertTowerParent" && __instance.name.ToLowerInvariant().Contains("ship"))
				return;

			// Allow small rocks to spawn underwater.
			if (parent == "G_ObjParent" && (name.Contains("rock") || name.StartsWith("ko")))
				return;

			// Destroy any objects under water.
			float height = __instance.transform.position.y - (float)mainscript.M.mainWorld.coord.y;
			if (height < WorldTweaker.Water.WaterHeight)
				GameObject.Destroy(__instance.gameObject);
		}
	}
}
