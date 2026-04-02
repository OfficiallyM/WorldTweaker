using HarmonyLib;
using System;
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
			float newLength = WorldTweaker.I.RoadLength.Value;
			__instance.scale = __instance.scale * (__instance.length / newLength);
			__instance.length = newLength;

			// Ensure road aligns correctly on flat world type.
			float worldType = WorldTweaker.I.WorldType.Value;
			if (worldType == 0)
			{
				__instance.doublePos = new doublecoord() { x = 0, y = 0.1, z = 0};
				__instance.roadAboveTerrain = 0.1f;
			}

			// Move road closer to starter house on road bridge world type.
			// This ensures you can actually leave the starter house.
			if (worldType == 1.25f)
				__instance.doublePos = new doublecoord() { x = 20, y = 0, z = 0 };
		}
	}

	[HarmonyPatch(typeof(roadGenScript), nameof(roadGenScript.PlaceOneRoad))]
	internal static class Patch_RoadGenScript_PlaceOneRoad
	{
		private static void Postfix(roadGenScript __instance, int pi)
		{
#if DEBUG
			if (pi == __instance.roadNum - 1)
			{
				// Don't spawn mom's house on really short roads.
				if (__instance.roadNum < 250) return;

				var road = __instance.roadList[pi];
				var bone = road.bones.Last();
				Vector3 pos = bone.position + bone.up * 30f + bone.right * 2f + bone.forward * -2f;
				GameObject house = GameObject.Instantiate(itemdatabase.d.bkezdohaz, pos, bone.rotation * Quaternion.Euler(0, 90f, -90f));
				house.transform.Find("Ramp").gameObject.SetActive(false);

				// Not happy with rotation but it'll do.
				GameObject.Instantiate(itemdatabase.d.gkezdolevel, pos + Vector3.up * 1f + Vector3.back * 3f, Quaternion.Euler(-70, 0, 90f));
			}
#endif

			if (WorldTweaker.I.WorldType.Value == 2f)
			{
				var road = __instance.roadList[pi];
				var roadData = __instance.roads[pi];
				if (road == null || roadData == null) return;

				if (road.doublePos.y > WorldTweaker.Water.WaterHeight + 1)
					return;

				var align = road.gameObject.AddComponent<terrainHeightAlignToBuildingScript>();
				var transforms = new Transform[roadData.coords.Length];
				align.range2 = new float[roadData.coords.Length];

				for (int i = 0; i < roadData.coords.Length; i++)
				{
					var helper = new GameObject("RoadAlignHelper");
					helper.transform.SetParent(__instance.transform, true);
					helper.transform.position = new Vector3(
						(float)(__instance.roads[pi].posX + __instance.world2.coord.x) + roadData.coords[i].x,
						(float)(__instance.roads[pi].posY + __instance.world2.coord.y) + roadData.coords[i].y,
						(float)(__instance.roads[pi].posZ + __instance.world2.coord.z) + roadData.coords[i].z);
					helper.transform.localScale = Vector3.one * 10f;
					transforms[i] = helper.transform;
					align.range2[i] = 8f;
				}

				align.helpPosCoords = transforms;
				align.FStart();
			}
		}
	}

	[HarmonyPatch(typeof(roadGenScript), nameof(roadGenScript.OneValue), new Type[] { typeof(float) })]
	internal static class Patch_RoadGenScript_OneValue
	{
		private static void Postfix(roadGenScript __instance, ref float __result, float pos)
		{
			__result *= WorldTweaker.I.RoadCurvature.Value;
		}
	}
}
