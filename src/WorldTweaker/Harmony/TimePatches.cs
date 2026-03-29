using HarmonyLib;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(napszakvaltakozas), "FixedUpdate")]
	internal static class Patch_napszakvaltakozas_FixedUpdate
	{
		private static void Postfix(napszakvaltakozas __instance)
		{
			// Alter world temperature based on world type.
			switch (WorldTweaker.I.WorldType.Value)
			{
				// Tropical.
				case 2f:
					__instance.temp -= 5f;
					break;
				// Lava.
				case 3f: 
					__instance.temp += 30f;
					break;
			}
		}
	}
}
