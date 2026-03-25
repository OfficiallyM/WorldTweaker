using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Lava : MonoBehaviour
	{
		public Material material;
		public static Lava ClosestLava;

		private static PostProcessVolume _lavaVolume;
		private static GameObject _playerSound;
		private static bool _isPlayerInLava;

		public void Start()
		{
			// Set to use water layer.
			gameObject.layer = 4;
			SetupLavaEffect();
		}

		private void SetupLavaEffect()
		{
			// Return early if already set up.
			if (_lavaVolume != null)
				return;

			var go = new GameObject("LavaPPV");
			go.layer = mainscript.M.player.DeadV.gameObject.layer;
			_lavaVolume = go.AddComponent<PostProcessVolume>();
			_lavaVolume.isGlobal = true;
			_lavaVolume.priority = 1f;

			var profile = ScriptableObject.CreateInstance<PostProcessProfile>();

			var colorGrading = profile.AddSettings<ColorGrading>();
			colorGrading.colorFilter.Override(new Color(0.2f, 0.1f, 0f));
			colorGrading.saturation.Override(-20f);

			_lavaVolume.profile = profile;
			_lavaVolume.weight = 0f;
		}

		public void LateUpdate()
		{
			var offset = material.mainTextureOffset + new Vector2(1f, 1f) * Time.deltaTime * 0.0001f;
			material.mainTextureOffset = new Vector2(
				Mathf.Repeat(offset.x, 1f),
				Mathf.Repeat(offset.y, 1f)
			);
		}

		public void Update()
		{
			fpscontroller player = mainscript.M.player;

			// Find the closest lava tile.
			Lava closest = null;
			float closestDist = float.MaxValue;
			foreach (var lava in WorldTweaker.Water.LavaTiles.Values)
			{
				float dist = Vector3.Distance(lava.transform.position, player.transform.position);
				if (dist < closestDist)
				{
					closestDist = dist;
					closest = lava;
				}
			}
			ClosestLava = closest;

			if (ClosestLava != this)
				return;

			// Set in lava camera effect.
			float depth = _isPlayerInLava ? Mathf.Max(0f, ClosestLava.transform.position.y - player.transform.position.y) : 0;
			_lavaVolume.weight = depth > 0.5f && !player.otherView() ? 1f : 0f;
		}

		public void OnTriggerStay(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;

			var rb = collider.attachedRigidbody;
			var player = mainscript.M.player;

			if (player.RB == rb)
			{
				if (!mainscript.M.godmode && !player.died)
				{
					float damage = player.survival.maxHp * 0.1f * Time.fixedDeltaTime;
					player.survival.DamageInstant(damage, true);

					if (_playerSound == null)
						_playerSound = mainscript.PlayClipAtPoint(WorldTweaker.Prefabs.Burn, player.transform.position, 0.2f);
				}
			}
			else
			{
				tosaveitemscript save = rb.gameObject.GetComponent<tosaveitemscript>();
				if (save == null)
					return;

				if (player.Car?.RB == rb)
					player.GetOut(player.transform.position, true);

				mainscript.PlayClipAtPoint(WorldTweaker.Prefabs.Burn, rb.transform.position, 0.2f);
				save.removeFromMemory = true;
				foreach (tosaveitemscript component in rb.transform.root.GetComponentsInChildren<tosaveitemscript>())
				{
					component.removeFromMemory = true;
				}
				GameObject.Destroy(rb.gameObject);
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;

			if (collider.attachedRigidbody == mainscript.M.player.RB)
				_isPlayerInLava = true;
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;

			if (collider.attachedRigidbody == mainscript.M.player.RB)
				_isPlayerInLava = false;
		}
	}
}
