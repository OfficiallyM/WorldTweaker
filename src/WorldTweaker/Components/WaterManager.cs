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

		public Mesh GenerateWaterMesh(float size, List<digholescript2> holes, Vector3 worldPos)
		{
			int resolution = 100;
			float cellSize = size / resolution;

			var vertices = new List<Vector3>();
			var triangles = new List<int>();
			var uvs = new List<Vector2>();

			for (int x = 0; x < resolution; x++)
			{
				for (int z = 0; z < resolution; z++)
				{
					// World space for hole checking
					float wx0 = worldPos.x - size / 2f + x * cellSize;
					float wx1 = wx0 + cellSize;
					float wz0 = worldPos.z - size / 2f + z * cellSize;
					float wz1 = wz0 + cellSize;

					if (IsInHole(wx0, wx1, wz0, wz1, holes))
						continue;

					// Local space for vertex positions (centered on object origin)
					float lx0 = -size / 2f + x * cellSize;
					float lx1 = lx0 + cellSize;
					float lz0 = -size / 2f + z * cellSize;
					float lz1 = lz0 + cellSize;

					int baseIndex = vertices.Count;

					vertices.Add(new Vector3(lx0, 0, lz0));
					vertices.Add(new Vector3(lx1, 0, lz0));
					vertices.Add(new Vector3(lx1, 0, lz1));
					vertices.Add(new Vector3(lx0, 0, lz1));

					uvs.Add(new Vector2((lx0 + size / 2f) / size, (lz0 + size / 2f) / size));
					uvs.Add(new Vector2((lx1 + size / 2f) / size, (lz0 + size / 2f) / size));
					uvs.Add(new Vector2((lx1 + size / 2f) / size, (lz1 + size / 2f) / size));
					uvs.Add(new Vector2((lx0 + size / 2f) / size, (lz1 + size / 2f) / size));

					triangles.Add(baseIndex);
					triangles.Add(baseIndex + 2);
					triangles.Add(baseIndex + 1);
					triangles.Add(baseIndex);
					triangles.Add(baseIndex + 3);
					triangles.Add(baseIndex + 2);
				}
			}

			var mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		private bool IsInHole(float x0, float x1, float z0, float z1, List<digholescript2> holes)
		{
			foreach (var hole in holes)
			{
				float hMinX = (float)System.Math.Min(System.Math.Min(hole.bal1.x, hole.bal2.x), System.Math.Min(hole.jobb1.x, hole.jobb2.x));
				float hMaxX = (float)System.Math.Max(System.Math.Max(hole.bal1.x, hole.bal2.x), System.Math.Max(hole.jobb1.x, hole.jobb2.x));
				float hMinZ = (float)System.Math.Min(System.Math.Min(hole.bal1.z, hole.bal2.z), System.Math.Min(hole.jobb1.z, hole.jobb2.z));
				float hMaxZ = (float)System.Math.Max(System.Math.Max(hole.bal1.z, hole.bal2.z), System.Math.Max(hole.jobb1.z, hole.jobb2.z));

				if (x1 > hMinX && x0 < hMaxX && z1 > hMinZ && z0 < hMaxZ)
					return true;
			}
			return false;
		}
	}
}
