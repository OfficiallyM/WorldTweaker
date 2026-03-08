using System.Collections.Generic;
using UnityEngine;

namespace WorldTweaker.Components
{
	internal class WaterManager : MonoBehaviour
	{
		public Transform WaterParent;
		public Dictionary<terrainscript, Water> DistantWater = new Dictionary<terrainscript, Water>();
	}
}
