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
			Destroy(transform.GetComponent<breakablescript>());
			Destroy(transform.GetComponent<materialChangeScript>());
			Destroy(transform.GetComponent<grassscript>());

			System.Random rng = new System.Random(transform.position.ToString("F3").GetHashCode());
			var prefab = WorldTweaker.Prefabs.PalmTrees[rng.Next(WorldTweaker.Prefabs.PalmTrees.Length)];
			var palm = Instantiate(prefab, transform, false);
			palm.transform.localPosition = Vector3.zero;
			palm.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			palm.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

			var rb = gameObject.GetComponent<Rigidbody>();
			if (rb == null)
				rb = gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;

			// Disable any default object children.
			foreach (Transform child in palm.transform.parent)
			{
				if (child == palm.transform) continue;
				child.gameObject.SetActive(false);
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
				var coconutObj = GameObject.Instantiate(WorldTweaker.Prefabs.Coconut, spawn.position, Quaternion.identity);
				var coconut = coconutObj.GetComponent<Coconut>();
				if (coconut == null)
					continue;
				coconut.Tree = this;
				Coconuts.Add(coconut);
			}
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (Coconuts.Count == 0)
				return;

			float targetMagnitude = 20f;
			if (collision.rigidbody == mainscript.M.player.RB)
				targetMagnitude = 5f;
			if (collision.relativeVelocity.magnitude > targetMagnitude)
			{
				// Player hit the tree themselves, drop a random coconut.
				if (targetMagnitude == 5f)
				{
					Coconuts[Random.Range(0, Coconuts.Count - 1)].Drop();
				}
				else
				{
					// Something else hit the tree with some force, drop all coconuts.
					foreach (var coconut in Coconuts)
					{
						coconut.Drop();
					}
				}
			}
		}
	}
}
