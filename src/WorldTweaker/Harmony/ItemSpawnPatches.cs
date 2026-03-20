using HarmonyLib;
using System;
using UnityEngine;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(newRandomStuffSpawnScript), nameof(newRandomStuffSpawnScript.Start))]
	internal static class Patch_newRandomStuffSpawnScript_Start
	{
		private static void Prefix(newRandomStuffSpawnScript __instance)
		{
			bool isMunkasSpawner = false;
			foreach (var item in __instance.items)
			{
				if (item.prefab == "humanmonster")
				{
					isMunkasSpawner = true;
					break;
				}
			}

			__instance.chanceToSpawn *= isMunkasSpawner
				? WorldTweaker.I.MunkasSpawnRate.Value
				: WorldTweaker.I.ItemSpawnRate.Value;
		}
	}

	[HarmonyPatch(typeof(newRandomStuffSpawnScript), nameof(newRandomStuffSpawnScript.Spawn))]
	internal static class Patch_newRandomStuffSpawnScript_Spawn
	{
		private static bool _isSpawningExtra = false;

		private static void Postfix(newRandomStuffSpawnScript __instance)
		{
			if (_isSpawningExtra)
				return;

			bool isMunkasSpawner = false;
			foreach (var item in __instance.items)
			{
				if (item.prefab == "humanmonster")
				{
					isMunkasSpawner = true;
					break;
				}
			}

			if (!isMunkasSpawner)
				return;

			float rate = WorldTweaker.I.MunkasSpawnRate.Value;
			int extraSpawns = Mathf.FloorToInt(rate) - 1;
			float remainder = rate - Mathf.Floor(rate);

			_isSpawningExtra = true;
			try
			{
				for (int i = 0; i < extraSpawns; i++)
					__instance.Spawn();

				if (remainder > 0f && UnityEngine.Random.value < remainder)
					__instance.Spawn();
			}
			finally
			{
				_isSpawningExtra = false;
			}
		}
	}

	[HarmonyPatch(typeof(undergroundSpawnScript), nameof(undergroundSpawnScript.Spawn))]
	internal static class Patch_undergroundSpawnScript_Spawn
	{
		private static void Prefix(undergroundSpawnScript __instance)
		{
			float rate = WorldTweaker.I.ItemSpawnRate.Value;
			__instance.chance *= rate;
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

	[HarmonyPatch(typeof(spawnInBoxScript), nameof(spawnInBoxScript.Spawn))]
	internal static class Patch_spawnInBoxScript_Spawn
	{
		private static int _originalMaxCount;
		private static int ScaleCount(int original)
		{
			// Leave 1-2 vanilla.
			if (original <= 2) return original;
			return Mathf.Max(2, (int)Math.Ceiling(original / 2f));
		}

		private static void Prefix(spawnInBoxScript __instance)
		{
			int modifier = WorldTweaker.I.CrateModifier.Value;
			if (modifier == 0)
			{
				// Always empty.
				__instance.disableSpawn = true;
			}
			else if (modifier == 2)
			{
				// Reduce amount.
				var mass = __instance.spawnObj?.GetComponent<massScript>();
				if (mass != null)
				{
					_originalMaxCount = mass.maxCountIfSpawnedInBox;
					mass.maxCountIfSpawnedInBox = ScaleCount(_originalMaxCount);
				}
			}
		}

		private static void Postfix(spawnInBoxScript __instance)
		{
			// Modify item conditions.
			int modifier = WorldTweaker.I.CrateModifier.Value;
			if (modifier != 2) return;

			// Restore vanilla spawn amount to avoid reducing indefinitely.
			var mass = __instance.spawnObj?.GetComponent<massScript>();
			if (mass != null)
				mass.maxCountIfSpawnedInBox = _originalMaxCount;

			if (__instance.mount == null) return;

			foreach (var stored in __instance.mount.stored)
			{
				stored.tosave.RandomSpawned(new newRandomStuffSpawnScript.item());
			}
		}
	}
}
