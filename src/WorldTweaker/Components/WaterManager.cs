using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class WaterManager : MonoBehaviour
	{
		public Transform WaterParent;
		public Dictionary<terrainscript, Water> WaterTiles = new Dictionary<terrainscript, Water>();
		public Dictionary<terrainscript, Lava> LavaTiles = new Dictionary<terrainscript, Lava>();
		public float WaterHeight
		{
			get
			{
				switch (WorldTweaker.I.WorldType.Value)
				{
					// Tropical.
					case 2f:
						return 449f;
					// Lava.
					case 3f: 
						return 999f;
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

		public Mesh GenerateWaterMesh(float size, List<digholescript2> holes, Vector3 worldPos, GameObject waterObj)
		{
			int resolution = 100;
			float cellSize = size / resolution;

			var vertices = new List<Vector3>();
			var triangles = new List<int>();
			var uvs = new List<Vector2>();
			bool[,] filled = new bool[resolution, resolution];

			for (int x = 0; x < resolution; x++)
			{
				for (int z = 0; z < resolution; z++)
				{
					// World space for hole checking.
					float wx0 = worldPos.x - size / 2f + x * cellSize;
					float wx1 = wx0 + cellSize;
					float wz0 = worldPos.z - size / 2f + z * cellSize;
					float wz1 = wz0 + cellSize;

					if (IsInHole(wx0, wx1, wz0, wz1, holes))
						continue;

					filled[x, z] = true;

					// Local space for vertex positions.
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

			AddMergedColliders(waterObj, filled, size, resolution);

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

		private void AddMergedColliders(GameObject go, bool[,] filled, float size, int resolution)
		{
			float cellSize = size / resolution;
			bool[,] processed = new bool[resolution, resolution];

			for (int x = 0; x < resolution; x++)
			{
				for (int z = 0; z < resolution; z++)
				{
					if (!filled[x, z] || processed[x, z])
						continue;

					// Expand as far as possible in Z.
					int zEnd = z;
					while (zEnd + 1 < resolution && filled[x, zEnd + 1] && !processed[x, zEnd + 1])
						zEnd++;

					// Expand as far as possible in X while keeping the full Z range.
					int xEnd = x;
					while (xEnd + 1 < resolution)
					{
						bool canExpand = true;
						for (int zi = z; zi <= zEnd; zi++)
						{
							if (!filled[xEnd + 1, zi] || processed[xEnd + 1, zi])
							{
								canExpand = false;
								break;
							}
						}
						if (!canExpand) break;
						xEnd++;
					}

					// Mark all cells in this rectangle as processed.
					for (int xi = x; xi <= xEnd; xi++)
						for (int zi = z; zi <= zEnd; zi++)
							processed[xi, zi] = true;

					// Create one collider for the whole rectangle.
					float lx0 = -size / 2f + x * cellSize;
					float lx1 = -size / 2f + (xEnd + 1) * cellSize;
					float lz0 = -size / 2f + z * cellSize;
					float lz1 = -size / 2f + (zEnd + 1) * cellSize;

					var col = go.AddComponent<BoxCollider>();
					col.isTrigger = true;
					col.center = new Vector3((lx0 + lx1) / 2f, -50000f, (lz0 + lz1) / 2f);
					col.size = new Vector3(lx1 - lx0, 100000f, lz1 - lz0);
				}
			}
		}
	}
}
