using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Water : MonoBehaviour
	{
		public Material material;

		public void LateUpdate()
		{
			material.mainTextureOffset += new Vector2(Mathf.PerlinNoise(Time.timeSinceLevelLoad * 0.1f, 0.0f) - 0.5f, Mathf.PerlinNoise(250f, Time.timeSinceLevelLoad * 0.1f) - 0.5f) * Time.deltaTime * 0.005f;
		}

		public void SetScale(float scale)
		{
			transform.localScale = new Vector3(scale, 0.01f, scale);
			material.mainTextureScale = new Vector2(scale / 10f, scale / 10f);
		}
	}
}
