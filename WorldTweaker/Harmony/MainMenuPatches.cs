using HarmonyLib;
using WorldTweaker.Core;
using WorldTweaker.Utilities;
using WorldTweaker.Utilities.UI;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(mainmenuscript), nameof(mainmenuscript.PressedLoadScene))]
	internal static class Patch_MainMenuScript_FStart
	{
		private static bool Prefix(mainmenuscript __instance, string s)
		{
			if (WorldTweaker.I.InterceptStart)
			{
				WorldTweaker.I.ShowUI = true;
				WorldTweaker.I.SetStartButtonState(false);
				Animator.Play("mainUI", Animator.AnimationState.SlideIn);
				return false;
			}
			return true;
		}
	}
}
