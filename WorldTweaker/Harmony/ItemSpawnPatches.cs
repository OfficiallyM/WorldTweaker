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
}
