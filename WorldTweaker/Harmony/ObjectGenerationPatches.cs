using HarmonyLib;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(ObjectGenerationScript), nameof(ObjectGenerationScript.FStart))]
	internal static class Patch_ObjectGenerationScript_FStart
	{
		private static void Prefix(ObjectGenerationScript __instance)
		{
			foreach (var obj in __instance.objTypes)
			{
				Logging.Log($"[PRE] Obj {obj.prefab.name}, chance: {obj.chance}");
				obj.chance *= WorldTweaker.I.ObjChanceFactor;
				Logging.Log($"[POST] Obj {obj.prefab.name}, chance: {obj.chance}");
			}
		}
	}
}
