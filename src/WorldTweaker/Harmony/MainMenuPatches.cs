using HarmonyLib;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(mainmenuscript), nameof(mainmenuscript.PressedLoadScene))]
	internal static class Patch_MainMenuScript_FStart
	{
		private static bool Prefix(mainmenuscript __instance, string s)
		{
			if (WorldTweaker.I.InterceptStart)
			{
				WorldTweaker.I.ToggleUI(true);
				return false;
			}
			return true;
		}
	}
}
