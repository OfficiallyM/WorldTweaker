using HarmonyLib;
using System.Linq;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(roadGenScript), nameof(roadGenScript.FStart))]
	internal static class Patch_RoadGenScript_FStart
	{
		private static void Prefix(roadGenScript __instance)
		{
			Logging.LogDebug($"Current length: {__instance.length} ({__instance.length / 1000}km)");
			float newLength = WorldTweaker.I.RoadLength.Value;
			__instance.scale = __instance.scale * (__instance.length / newLength);
			__instance.length = newLength;
			Logging.LogDebug($"New length: {__instance.length} ({__instance.length / 1000}km)");
		}
	}

	[HarmonyPatch(typeof(roadGenScript), nameof(roadGenScript.PlaceOneRoad))]
	internal static class Patch_RoadGenScript_PlaceOneRoad
	{
		private static void Postfix(roadGenScript __instance, int pi)
		{
			if (pi == __instance.roadNum - 1)
			{
				// Don't spawn mom's house on really short roads.
				if (__instance.roadNum < 250) return;

				var road = __instance.roadList[pi];
				var bone = road.bones.Last();
				Vector3 pos = bone.position + bone.up * 30f + bone.right * 2f;
				GameObject house = GameObject.Instantiate(itemdatabase.d.bkezdohaz, pos, bone.rotation * Quaternion.Euler(0, 90f, -90f));
				house.transform.Find("Ramp").gameObject.SetActive(false);

				// Not happy with rotation but it'll do.
				GameObject.Instantiate(itemdatabase.d.gkezdolevel, pos + Vector3.up * 1f + Vector3.back * 3f, Quaternion.Euler(-70, 0, 90f));
			}
		}
	}
}
