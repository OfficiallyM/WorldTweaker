using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Water : MonoBehaviour
	{
		public Material material;

		public static bool IsPlayerSwimming
		{
			get
			{
				return _playerDepth > 0.4f;
			}
		}

		public static bool IsPlayerDrowning
		{
			get
			{
				return _playerDepth > 0.7f;
			}
		}

		private const float DrownRecoveryDelay = 3f;

		private static Slider _drown;
		private static float _playerDepth;
		private static float _drownRecoveryTime = -1f;

		private Dictionary<Rigidbody, (
			float drag,
			float angularDrag,
			float jumpForce
		)> _originalValues = new Dictionary<Rigidbody, (float, float, float)>();

		public void OnGUI()
		{
			if (WorldTweaker.Debug) return;
			var rb = mainscript.M.player?.RB;
			GUI.Button(new Rect(0, 0, 300, 30), $"Depth: {_playerDepth}");
			GUI.Button(new Rect(0, 35, 300, 30), $"Swimming: {IsPlayerSwimming}");
		}

		public void LateUpdate()
		{
			material.mainTextureOffset += new Vector2(
				Mathf.PerlinNoise(Time.timeSinceLevelLoad * 0.1f, 0.0f) - 0.5f,
				Mathf.PerlinNoise(250f, Time.timeSinceLevelLoad * 0.1f) - 0.5f
			) * Time.deltaTime * 0.005f;

			// Keep original values dictionary clean.
			foreach (var key in new List<Rigidbody>(_originalValues.Keys))
				if (key == null)
					_originalValues.Remove(key);

			// Disable gravity when swimming.
			fpscontroller player = mainscript.M.player;
			if (IsPlayerSwimming && player.RB != null)
				player.RB.useGravity = false;

			// Reduce drown meter when out of water.
			if (_drown != null)
			{
				if (IsPlayerDrowning)
				{
					// Reset while still drowning.
					_drownRecoveryTime = -1f;
				}
				else
				{
					if (_drownRecoveryTime < 0f)
						_drownRecoveryTime = Time.time + DrownRecoveryDelay;

					if (Time.time >= _drownRecoveryTime)
					{
						_drown.value -= 0.004f * Time.deltaTime;

						if (_drown.value <= 0f)
							_drown.gameObject.SetActive(false);
					}
				}
			}
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
			if (_originalValues.ContainsKey(rb))
				return;
			var player = mainscript.M.player;
			_originalValues[rb] = (rb.drag, rb.angularDrag, player.RB == rb ? player.FjumpForce : 0);
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;
			var rb = collider.attachedRigidbody;
			var player = mainscript.M.player;
			if (_originalValues.TryGetValue(rb, out var original))
			{
				rb.drag = original.drag;
				rb.angularDrag = original.angularDrag;
				if (player.RB == rb)
					player.FjumpForce = original.jumpForce;
				_originalValues.Remove(rb);
			}
			if (player.RB == rb)
			{
				_playerDepth = 0;
				player.RB.useGravity = true;
			}
		}

		public void OnTriggerStay(Collider collider)
		{
			if (collider.attachedRigidbody == null)
				return;

			var rb = collider.attachedRigidbody;
			float depth = Mathf.Max(0f, transform.position.y - rb.transform.position.y);

			UpdateDrag(rb, depth);
			UpdatePlayer(rb, depth);
			UpdateTanks(rb, depth);
		}

		/// <summary>
		/// Handle application of rigidbody drag when in water.
		/// </summary>
		/// <param name="rb">Rigidbody in water</param>
		/// <param name="depth">Current water depth</param>
		private void UpdateDrag(Rigidbody rb, float depth)
		{
			// Return early if the original values aren't cached
			// to prevent the water values from caching.
			if (!_originalValues.ContainsKey(rb))
				return;

			rb.drag = Mathf.Max(2f, depth / 6f);
			rb.angularDrag = Mathf.Max(2f, depth / 6f);

			// Don't apply any forces to the player.
			if (mainscript.M.player.RB == rb)
				return;

			rb.AddForce(Vector3.up * 50f * depth / rb.mass);
		}

		/// <summary>
		/// Handle player swimming.
		/// </summary>
		/// <param name="rb">Rigidbody in water</param>
		/// <param name="depth">Current water depth</param>
		private void UpdatePlayer(Rigidbody rb, float depth)
		{
			fpscontroller player = mainscript.M.player;
			if (player.RB != rb)
				return;

			_playerDepth = depth;
			
			if (IsPlayerSwimming)
			{
				player.duckValue = 5f;
				player.lastDucked = false;
				player.Bduck = false;

				// Reduce vertical velocity when not intentionally changing Y.
				if (!player.input.duck && !player.input.jump)
					rb.velocity = new Vector3(
						rb.velocity.x,
						Mathf.Lerp(rb.velocity.y, 0f, Time.fixedDeltaTime * 5f),
						rb.velocity.z
					);

				// Zero out jump force so the vanilla jump doesn't interfere.
				if (_originalValues.ContainsKey(rb))
					player.FjumpForce = 0f;
			}
			else if (_originalValues.TryGetValue(rb, out var original))
			{
				// Restore jump force when wading but not swimming.
				player.FjumpForce = original.jumpForce;
			}

			if (player.Car == null)
			{
				float swimForce = Mathf.Max(10f, 10f * depth);
				if (player.input.jump)
				{
					player.RB.AddForce(Vector3.up * swimForce);
				}

				if (player.input.duck)
				{
					player.RB.AddForce(Vector3.down * 60f);
				}
			}

			if (IsPlayerDrowning)
			{
				_drown = mainscript.M.menu.TTankCap.transform.GetComponentInChildren<Slider>();
				if (_drown == null)
				{
					_drown = Instantiate(mainscript.M.menu.SPiss, mainscript.M.menu.TTankCap.transform, false);
					_drown.transform.Find("Fill Area").GetComponentInChildren<Image>().color = Color.cyan;
					_drown.value = 0.0f;
					_drown.maxValue = 1f;
					_drown.fillRect.localScale = new Vector3(1f, 1f, 0f);
					_drown.enabled = true;
					_drown.gameObject.SetActive(true);
					_drown.transform.Find("ImagePiss").gameObject.SetActive(false);
				}
				else
				{
					_drown.gameObject.SetActive(true);
					_drown.value += 0.0015f * Time.fixedDeltaTime;
					mainscript.M.player.deathSens = 0.5f;

					Image slide = _drown.transform.Find("Fill Area").GetComponentInChildren<Image>();
					slide.color = Color.cyan;
					if (_drown.value > 0.75f)
						slide.color = Color.red;

					if (_drown.value >= 1f && !mainscript.M.died)
						mainscript.M.player.Death();
				}
			}
		}

		/// <summary>
		/// Fill any open tanks when under water.
		/// </summary>
		/// <param name="rb">Rigidbody in water</param>
		/// <param name="depth">Current water depth</param>
		private void UpdateTanks(Rigidbody rb, float depth)
		{
			float fillRate = 0.5f * depth * Time.fixedDeltaTime;

			foreach (var tank in rb.transform.root.GetComponentsInChildren<tankscript>())
			{
				if (tank.F.GetAmount() >= tank.F.maxC) 
					continue;

				bool open = false;
				foreach (var cap in tank.TC)
				{
					if (cap.valve > 0 && cap.TCap.position.y < transform.position.y)
					{
						open = true;
						break;
					}
				}

				if (!open) 
					continue;

				tank.F.ChangeOne(fillRate, mainscript.fluidenum.water);
			}
		}
	}
}