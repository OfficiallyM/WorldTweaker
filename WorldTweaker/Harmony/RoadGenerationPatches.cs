using HarmonyLib;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(roadGenScript), nameof(roadGenScript.FStart))]
	internal static class Patch_RoadGenScript_FStart
	{
		private static void Prefix(roadGenScript __instance)
		{
			Logging.LogDebug($"Current length: {__instance.length} ({__instance.length / 1000}km)");
			__instance.length = WorldTweaker.I.RoadLength;
			Logging.LogDebug($"New length: {__instance.length} ({__instance.length / 1000}km)");
		}
	}
}
