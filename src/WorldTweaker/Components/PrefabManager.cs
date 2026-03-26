using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class PrefabManager : MonoBehaviour
	{
		public GameObject Water;
		public GameObject Lava;
		public Shader WaterShader;
		public Texture WaterTexture;
		public Material LavaMaterial;
		public GameObject[] PalmTrees;
		public AudioClip Burn;

		public void CreatePrefabs()
		{
			// Water.
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
			Water.AddComponent<Water>().material = material;

			// Lava.
			Lava = new GameObject("Lava");
			Lava.transform.position = Vector3.negativeInfinity;
			originShift = Lava.AddComponent<visszarako>();
			originShift.moveWhenParented = true;
			Lava.AddComponent<MeshFilter>().mesh = itemdatabase.d.gerror.GetComponentInChildren<MeshFilter>().mesh;
			renderer = Lava.AddComponent<MeshRenderer>();
			var lavaMaterial = new Material(LavaMaterial);
			lavaMaterial.mainTextureScale = new Vector2(200f, 200f);
			renderer.material = lavaMaterial;
			Lava.AddComponent<Lava>().material = lavaMaterial;
		}
	}
}
