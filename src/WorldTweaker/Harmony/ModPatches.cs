using HarmonyLib;
using UnityEngine;

namespace WorldTweaker.Harmony
{
	internal static class Patch_SgtJoeBuildingBase_ParseItemFromSpawnString
	{
		public static void Postfix(ref int spawnChance)
		{
			float rate = WorldTweaker.I.ItemSpawnRate.Value;
			if (rate == 0)
			{
				spawnChance = -1;
				return;
			}

			spawnChance = Mathf.Clamp((int)(spawnChance * rate), 0, 100);
		}
	}

	internal static class Patch_WeatherScript_ChooseNewWeather
	{
		public static void Postfix(object __instance)
		{
			// Only change weather for lava world.
			if (WorldTweaker.I.WorldType.Value != 3f)
				return;

			var activeWeather = Traverse.Create(__instance).Field("ActiveWeather").GetValue<int>();

			// 1 = Rain, 2 = Snow, 4 = Lightning, 6 = Snowstorm.
			if (activeWeather == 1 || activeWeather == 2 || activeWeather == 4 || activeWeather == 6)
				Traverse.Create(__instance).Field("ActiveWeather").SetValue(0);
		}
	}
}
