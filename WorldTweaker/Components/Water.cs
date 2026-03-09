using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Water : MonoBehaviour
	{
		public Material material;

		private Dictionary<Rigidbody, (float drag, float angularDrag)> _originalDrag = new Dictionary<Rigidbody, (float, float)>();

		public void LateUpdate()
		{
			material.mainTextureOffset += new Vector2(Mathf.PerlinNoise(Time.timeSinceLevelLoad * 0.1f, 0.0f) - 0.5f, Mathf.PerlinNoise(250f, Time.timeSinceLevelLoad * 0.1f) - 0.5f) * Time.deltaTime * 0.005f;

			// Keep drag dict clean.
			foreach (var key in new List<Rigidbody>(_originalDrag.Keys))
				if (key == null)
					_originalDrag.Remove(key);
		}

		public void SetScale(float scale)
		{
			transform.localScale = new Vector3(scale, 0.01f, scale);
			material.mainTextureScale = new Vector2(scale / 10f, scale / 10f);
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;
			var rb = collider.attachedRigidbody;
			if (_originalDrag.ContainsKey(rb))
				return;

			if (rb == mainscript.M.player.RB)
				Logging.LogDebug($"Stored drag: {rb.drag}, angular drag: {rb.angularDrag}");

			_originalDrag[rb] = (rb.drag, rb.angularDrag);
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;
			var rb = collider.attachedRigidbody;
			if (_originalDrag.TryGetValue(rb, out var original))
			{
				if (rb == mainscript.M.player.RB)
					Logging.LogDebug($"Restored drag: {original.drag}, angular drag: {original.angularDrag}");
				rb.drag = original.drag;
				rb.angularDrag = original.angularDrag;
				_originalDrag.Remove(rb);
			}
		}

		public void OnTriggerStay(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;

			var rb = collider.attachedRigidbody;
			float depth = Mathf.Max(0f, transform.position.y - collider.transform.position.y);

			UpdateDrag(rb, depth);
		}

		private void UpdateDrag(Rigidbody rb, float depth)
		{
			// Return early if the original drag values aren't cached
			// to prevent the water values from caching.
			if (!_originalDrag.ContainsKey(rb))
				return;

			if (mainscript.M.player.RB == rb)
			{
				rb.AddForce(Vector3.up * 100f * depth);
				rb.drag = Mathf.Max(3f, depth);
				rb.angularDrag = Mathf.Max(3f, depth);
				return;
			}

			rb.AddForce(Vector3.up * 100f * depth / rb.mass);
			rb.drag = Mathf.Max(3f, depth);
			rb.angularDrag = Mathf.Max(3f, depth);
		}
	}
}
