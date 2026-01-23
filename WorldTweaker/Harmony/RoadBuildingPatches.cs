using HarmonyLib;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(roadBuildingScript), nameof(roadBuildingScript.FStart))]
	internal static class Patch_RoadBuildingScript_FStart
	{
		private static void Prefix(roadBuildingScript __instance)
		{
			float density = WorldTweaker.I.BuildingDensity.Value;

			if (density == 0)
			{
				__instance.generate = false;
			}
			else
			{
				float multiplier = 1f / Mathf.Sqrt(density);

				__instance.minStartDist *= multiplier;
				__instance.maxStartDist *= multiplier;

				foreach (var group in __instance.groups)
				{
					foreach (var type in group.types)
					{
						type.minDist *= multiplier;
						type.maxDist *= multiplier;
					}
				}
			}
		}
	}
}
