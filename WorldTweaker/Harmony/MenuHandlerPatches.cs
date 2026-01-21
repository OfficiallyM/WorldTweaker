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

			WorldData worldData = Save.GetWorldData();
			if (worldData != null)
			{
				WorldTweaker.I.RoadLength = worldData.Length;
				WorldTweaker.I.ObjChanceFactor = worldData.ObjChanceFactor;
			}
			else
			{
				WorldTweaker.I.RoadLength = 5000000;
				WorldTweaker.I.ObjChanceFactor = 1f;
			}
		}
	}
}
