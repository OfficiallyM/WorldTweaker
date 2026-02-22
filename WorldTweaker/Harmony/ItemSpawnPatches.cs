using HarmonyLib;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(newRandomStuffSpawnScript), nameof(newRandomStuffSpawnScript.Start))]
	internal static class Patch_newRandomStuffSpawnScript_Start
	{
		private static void Prefix(newRandomStuffSpawnScript __instance)
		{
			float rate = WorldTweaker.I.ItemSpawnRate.Value;
			__instance.chanceToSpawn *= rate;
		}
	}

	[HarmonyPatch(typeof(tankscript), nameof(tankscript.RandomFluid))]
	internal static class Patch_tankscript_RandomFluid
	{
		private static void Prefix(tankscript __instance)
		{
			float amount = WorldTweaker.I.FluidAmount.Value;
			__instance.minFluid *= amount;
			__instance.maxFluid *= amount;
		}
	}
}
