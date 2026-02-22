using HarmonyLib;
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
}
