using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class Coconut : MonoBehaviour
	{
		public PalmTree Tree;
		private Rigidbody _rb;
		private tosaveitemscript _toSave;

		public void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_toSave = gameObject.GetComponent<tosaveitemscript>();

			if (Tree != null)
			{
				_rb.useGravity = false;
				Destroy(_toSave);
				_toSave = null;
			}
		}

		public void Drop()
		{
			_rb.useGravity = true;
			Tree.Coconuts.Remove(this);
			Tree = null;
			WorldTweaker.Prefabs.MakeSavable(gameObject, 0)?.FStart();
		}
	}
}
