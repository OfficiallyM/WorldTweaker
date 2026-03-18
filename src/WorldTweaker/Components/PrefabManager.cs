using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class PrefabManager : MonoBehaviour
	{
		public GameObject Water;
		public Shader WaterShader;
		public Texture WaterTexture;

		public void CreatePrefabs()
		{
			Water = new GameObject("Water");
			Water.transform.position = Vector3.negativeInfinity;
			var originShift = Water.AddComponent<visszarako>();
			originShift.moveWhenParented = true;
			Water.AddComponent<MeshFilter>().mesh = itemdatabase.d.gerror.GetComponentInChildren<MeshFilter>().mesh;
			var renderer = Water.AddComponent<MeshRenderer>();
			var material = new Material(WaterShader);
			material.mainTexture = WaterTexture;
			material.color = new Color(0.6f, 0.9f, 1f, 0.75f);
			renderer.material = material;
			var collider = Water.AddComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = new Vector3(1f, 100000f, 1f);
			collider.center = new Vector3(0.0f, -50000f, 0.0f);
			Water.AddComponent<Water>().material = material;
		}
	}
}
