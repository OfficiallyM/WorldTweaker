using System.Collections.Generic;

namespace WorldTweaker.Core
{
	internal enum QueueType
	{
		upsert,
		delete,
	}

	internal sealed class ToDelete : Savable { }

	internal sealed class QueueEntry
	{
		public QueueType QueueType { get; set; } = QueueType.upsert;
		public Savable Data { get; set; }
	}

	internal abstract class Savable
	{
		public string Type { get; set; }

		public Savable()
		{
			Type = GetType().Name;
		}
	}

	internal sealed class SaveData
	{
		public List<Savable> Data { get; set; }
	}

	internal class WorldData : Savable
	{
		public float Length { get; set; }
		public float ObjectDensity { get; set; }
		public float MountainDensity { get; set; }
		public float BuildingDensity { get; set; }

		public WorldData(float length, float objectDensity, float mountainDensity, float buildingDensity)
		{
			Length = length;
			ObjectDensity = objectDensity;
			MountainDensity = mountainDensity;
			BuildingDensity = buildingDensity;
		}
	}
}
