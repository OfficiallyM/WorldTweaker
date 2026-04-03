using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Coconut : MonoBehaviour
	{
		public PalmTree Tree;
		private Rigidbody _rb;
		private pickupable _pickup;

		public void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_pickup = GetComponent<pickupable>();

			if (Tree != null)
			{
				_rb.useGravity = false;
				_rb.isKinematic = true;
			}
		}

		public void Update()
		{
			if (Tree != null && _pickup.pickedUp)
				Drop();
		}

		public void Drop()
		{
			transform.SetParent(null, true);
			_rb.useGravity = true;
			_rb.isKinematic = false;
			Tree.Coconuts.Remove(this);
			Tree = null;
			WorldTweaker.Prefabs.MakeSavable(gameObject, 0)?.FStart();
		}
	}
}
