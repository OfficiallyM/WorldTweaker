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
}
