using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class WaterManager : MonoBehaviour
	{
		public Transform WaterParent;
		public Dictionary<terrainscript, Water> DistantWater = new Dictionary<terrainscript, Water>();
		public float WaterHeight
		{
			get
			{
				switch (WorldTweaker.I.WorldType.Value)
				{
					case 2f:
						return 449f;
					default:
						return 0;
				}
			}
		}

		public void Start()
		{
			// Add water layer to far camera culling mask to ensure water
			// in the distance is rendered.
			mainscript.M.player.farCamera.cullingMask |= (1 << 4);
		}
	}
}
