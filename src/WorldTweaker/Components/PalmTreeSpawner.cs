using UnityEngine;

namespace WorldTweaker.Components
{
	internal class PalmTreeSpawner : MonoBehaviour
	{
		public void Start()
		{
			var prefab = WorldTweaker.Prefabs.PalmTrees[Random.Range(0, WorldTweaker.Prefabs.PalmTrees.Length)];
			var palm = Instantiate(prefab, transform, false);
			palm.transform.localPosition = Vector3.zero;
			palm.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			palm.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

			foreach (Transform child in palm.transform.parent)
			{
				if (child == palm.transform) continue;
				child.gameObject.SetActive(false);
			}
		}
	}
}
