using System.Collections.Generic;
using UnityEngine;

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
	}
}
