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

			WorldTweaker.I.RoadLength.SetValue(worldData?.Length ?? 5000000);
			WorldTweaker.I.RoadCurvature.SetValue(worldData?.RoadCurvature ?? 1f);
			WorldTweaker.I.ObjectDensity.SetValue(worldData?.ObjectDensity ?? 1f);
			WorldTweaker.I.MountainDensity.SetValue(worldData?.MountainDensity ?? 1f);
			WorldTweaker.I.BuildingDensity.SetValue(worldData?.BuildingDensity ?? 1f);
			WorldTweaker.I.WorldType.SetValue(worldData?.WorldType ?? 1f);
			WorldTweaker.I.ItemSpawnRate.SetValue(worldData?.ItemSpawnRate ?? 1f);
			WorldTweaker.I.FluidAmount.SetValue(worldData?.FluidAmount ?? 1f);
			WorldTweaker.I.CrateModifier.SetValue(worldData?.CrateModifier ?? 1);
		}
	}
}
