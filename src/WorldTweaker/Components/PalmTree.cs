using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class PalmTree : MonoBehaviour
	{
		public List<Coconut> Coconuts = new List<Coconut>();

		public void Start()
		{
			if (WorldTweaker.I.WorldType.Value != 2f)
				return;

			Destroy(transform.GetComponent<breakablescript>());
			Destroy(transform.GetComponent<materialChangeScript>());
			Destroy(transform.GetComponent<grassscript>());

			var worldOffset = new Vector3(
				(float)mainscript.M.mainWorld.coord.x,
				(float)mainscript.M.mainWorld.coord.y,
				(float)mainscript.M.mainWorld.coord.z
			);
			var seed = Hash.FNV1((transform.position - worldOffset).ToString("F2"));
			System.Random rng = new System.Random(seed);
			var prefab = WorldTweaker.Prefabs.PalmTrees[rng.Next(WorldTweaker.Prefabs.PalmTrees.Length)];
			var palm = Instantiate(prefab, transform, false);
			palm.transform.localPosition = Vector3.zero;
			palm.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			palm.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

			// Disable any default object children.
			foreach (Transform child in palm.transform.parent)
			{
				if (child == palm.transform) continue;
				child.gameObject.SetActive(false);
			}

			// Handle tree impacts.
			foreach (var collider in palm.GetComponentsInChildren<Collider>())
			{
				//collider.isTrigger = true;
				var listener = collider.gameObject.AddComponent<PalmTreeImpactListener>();
				listener.Tree = this;
			}

			// Coconut spawns.
			List<Transform> coconutSpawns = new List<Transform>();
			foreach (Transform child in palm.transform)
			{
				if (child.name.Contains("CoconutSpawn"))
					coconutSpawns.Add(child);
			}

			int coconutCount = rng.Next(coconutSpawns.Count);
			if (coconutCount == 0)
				return;

			for (int i = 0; i < coconutCount; i++)
			{
				Transform spawn = coconutSpawns[i];
				var coconutObj = GameObject.Instantiate(WorldTweaker.Prefabs.Coconut);
				coconutObj.transform.SetParent(spawn);
				coconutObj.transform.localRotation = Quaternion.identity;
				coconutObj.transform.localPosition = Vector3.zero;
				Destroy(coconutObj.GetComponent<tosaveitemscript>());
				var coconut = coconutObj.GetComponent<Coconut>();
				if (coconut == null)
					continue;
				coconut.Tree = this;
				Coconuts.Add(coconut);
			}
		}

		public void OnImpact(Rigidbody hitRb)
		{
			if (Coconuts.Count == 0)
				return;

			bool isPlayer = hitRb == mainscript.M.player.RB;
			float speed = hitRb.velocity.magnitude;
			float targetMagnitude = isPlayer ? 5f : 20f;

			if (speed > targetMagnitude)
			{
				if (isPlayer)
				{
					Coconuts[Random.Range(0, Coconuts.Count - 1)].Drop();
				}
				else
				{
					foreach (var coconut in Coconuts)
						coconut.Drop();
				}
			}
		}
	}

	internal class PalmTreeImpactListener : MonoBehaviour
	{
		public PalmTree Tree;

		private void OnTriggerEnter(Collider other)
		{
			var rb = other.attachedRigidbody;
			if (rb == null) return;

			Tree.OnImpact(rb);
		}
	}
}
