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
				WorldTweaker.I.RoadLength.SetValue(worldData.Length);
				WorldTweaker.I.ObjectDensity.SetValue(worldData.ObjectDensity);
				WorldTweaker.I.MountainDensity.SetValue(worldData.MountainDensity);
				WorldTweaker.I.BuildingDensity.SetValue(worldData.BuildingDensity);
			}
			else
			{
				WorldTweaker.I.RoadLength.SetValue(5000000);
				WorldTweaker.I.ObjectDensity.SetValue(1f);
				WorldTweaker.I.MountainDensity.SetValue(1f);
				WorldTweaker.I.BuildingDensity.SetValue(1f);
			}
		}
	}
}
