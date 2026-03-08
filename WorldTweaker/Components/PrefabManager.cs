using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class PrefabManager : MonoBehaviour
	{
		public GameObject Water;

		public void CreatePrefabs()
		{
			var texture = TextureUtilities.LoadTexture("water.png");
			Water = new GameObject("Water");
			Water.transform.position = Vector3.negativeInfinity;
			Water.transform.localScale = new Vector3(100f, 0.01f, 100f);
			var originShift = Water.AddComponent<visszarako>();
			originShift.moveWhenParented = true;
			Water.AddComponent<MeshFilter>().mesh = itemdatabase.d.gerror.GetComponentInChildren<MeshFilter>().mesh;
			var renderer = Water.AddComponent<MeshRenderer>();
			renderer.material = new Material(Shader.Find("Mobile/Particles/Multiply"));
			renderer.material.mainTexture = texture;
			renderer.material.mainTextureScale = new Vector2(10f, 10f);
			renderer.material.color = new Color(1f, 1f, 1f, 0.25f);
			var collider = Water.AddComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = new Vector3(1f, 100000f, 1f);
			collider.center = new Vector3(0.0f, -50000f, 0.0f);
			Water.AddComponent<Water>().material = renderer.material;
		}
	}
}
