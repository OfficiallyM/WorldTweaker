using HarmonyLib;
using WorldTweaker.Core;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(menuhandler), nameof(menuhandler.FStart))]
	internal static class Patch_MenuHandler_FStart
	{
		private static void Postfix(menuhandler __instance)
		{
			// Don't try loading save data for new games.
			if (!__instance.DFMS.load) return;

			RoadData roadData = Save.GetRoadData();
			if (roadData != null)
			{
				WorldTweaker.I.RoadLength = roadData.Length;
			}
			else
			{
				WorldTweaker.I.RoadLength = 5000000;
			}
		}
	}
}
