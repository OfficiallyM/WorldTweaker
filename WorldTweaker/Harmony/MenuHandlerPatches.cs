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
			var wt = WorldTweaker.I;
			if (mainscript.M.load || (mainscript.M.DFMS?.load ?? false))
			{
				// Load save data.
				WorldData worldData = Save.GetWorldData();

				Logging.LogDebug("Loading from save data");
				Logging.LogDebug($"Save data: {(worldData?.ToString() ?? "null")}");

				wt.RoadLength.SetValue(worldData?.Length ?? 5000000);
				wt.RoadCurvature.SetValue(worldData?.RoadCurvature ?? 1f);
				wt.ObjectDensity.SetValue(worldData?.ObjectDensity ?? 1f);
				wt.MountainDensity.SetValue(worldData?.MountainDensity ?? 1f);
				wt.BuildingDensity.SetValue(worldData?.BuildingDensity ?? 1f);
				wt.WorldType.SetValue(worldData?.WorldType ?? 1f);
				wt.ItemSpawnRate.SetValue(worldData?.ItemSpawnRate ?? 1f);
				wt.FluidAmount.SetValue(worldData?.FluidAmount ?? 1f);
				wt.CrateModifier.SetValue(worldData?.CrateModifier ?? 1);
			}

			// Force tropical to desert biome.
			if (wt.WorldType.Value == 2f)
			{
				mainscript.M.GSettings.biome = 1;
			}

			Logging.LogDebug($"RoadLength: {wt.RoadLength}");
			Logging.LogDebug($"RoadCurvature: {wt.RoadCurvature}");
			Logging.LogDebug($"ObjectDensity: {wt.ObjectDensity}");
			Logging.LogDebug($"MountainDensity: {wt.MountainDensity}");
			Logging.LogDebug($"BuildingDensity: {wt.BuildingDensity}");
			Logging.LogDebug($"WorldType: {wt.WorldType}");
			Logging.LogDebug($"ItemSpawnRate: {wt.ItemSpawnRate}");
			Logging.LogDebug($"FluidAmount: {wt.FluidAmount}");
			Logging.LogDebug($"CrateModifier: {wt.CrateModifier}");
		}
	}
}
