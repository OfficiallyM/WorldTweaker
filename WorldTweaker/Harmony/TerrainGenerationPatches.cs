using HarmonyLib;
using System;
using UnityEngine;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.GetTerrainHeight),
	new Type[] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
	public static class GetTerrainHeight_Postfix
	{
		public static void Postfix(ref float __result)
		{
			float flatness = WorldTweaker.I.WorldType.Value;
			if (flatness == 0)
			{
				__result = 0;
				return;
			}

			__result *= flatness;
		}
	}
}
