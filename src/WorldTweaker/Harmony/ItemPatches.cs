using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Harmony
{
	[HarmonyPatch(typeof(itemdatabase), nameof(itemdatabase.ReturnItem))]
	internal static class Patch_itemdatabase_ReturnItem
	{
		private static void Postfix(itemdatabase __instance, ref GameObject __result, int category, int id)
		{
			if (category != WorldTweaker.Category)
				return;

			switch (id)
			{
				case 0:
					__result = WorldTweaker.Prefabs.Coconut;
					break;
			}
		}
	}

	[HarmonyPatch(typeof(itemPlaceRemoveScript), nameof(itemPlaceRemoveScript.PlaceOneItem))]
	internal static class Patch_itemPlaceRemoveScript_PlaceOneItem
	{
		private static bool Prefix(itemPlaceRemoveScript __instance, KeyValuePair<int, save_prefab> _t)
		{
			if (_t.Value.categoryID != WorldTweaker.Category)
				return true;

			var spawnedThisFrame = Traverse.Create(__instance).Field("spawnedThisFrame").GetValue<List<int>>();
			if (spawnedThisFrame == null) return false;

			var tempGameObjs = Traverse.Create(__instance).Field("tempGameObjs").GetValue<List<GameObject>>();
			if (tempGameObjs == null) return false;

			++__instance.db;
			bool flag = false;
			Vector3 _pos = Vector3.zero;
			__instance.settings.CheckImportantObjsNull();

			float dist = 0;

			foreach (ImportantObj importantObj in __instance.settings.importantObjs)
			{
				_pos = new Vector3((float)(mainscript.M.mainWorld.coord.x + _t.Value.transform.pos.x), (float)(mainscript.M.mainWorld.coord.y + _t.Value.transform.pos.y), (float)(mainscript.M.mainWorld.coord.z + _t.Value.transform.pos.z));
				dist = (new Vector2(importantObj.T.position.x, importantObj.T.position.z) - new Vector2(_pos.x, _pos.z)).sqrMagnitude;
				if ((double)dist < (double)__instance.itemSpawnDist)
				{
					flag = true;
					break;
				}
			}
			if (!flag || __instance.save.toSaveStuff.ContainsKey(_t.Key) || spawnedThisFrame.Contains(_t.Key))
				return false;
			spawnedThisFrame.Add(_t.Key);
			Traverse.Create(__instance).Field("spawnedThisFrame").SetValue(spawnedThisFrame);
			__instance.toRemove.Add(_t.Key);
			tempGameObjs.Add(__instance.save.SpawnItem(_t.Value, _pos, _t.Key));
			Traverse.Create(__instance).Field("tempGameObjs").SetValue(tempGameObjs);
			return false;
		}
	}
}
