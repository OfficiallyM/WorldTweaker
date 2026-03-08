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
		public string Version { get; set; }
		public float? Length { get; set; }
		public float? RoadCurvature { get; set; }
		public float? ObjectDensity { get; set; }
		public float? MountainDensity { get; set; }
		public float? BuildingDensity { get; set; }
		public float? WorldType { get; set; }
		public float? ItemSpawnRate { get; set; }
		public float? FluidAmount { get; set; }
		public int? CrateModifier { get; set; }

		public WorldData(
			float? length,
			float? roadCurvature,
			float? objectDensity,
			float? mountainDensity,
			float? buildingDensity,
			float? worldType,
			float? itemSpawnRate,
			float? fluidAmount,
			int? crateModifier
		)
		{
			Version = WorldTweaker.I.Version;
			Length = length;
			RoadCurvature = roadCurvature;
			ObjectDensity = objectDensity;
			MountainDensity = mountainDensity;
			BuildingDensity = buildingDensity;
			WorldType = worldType;
			ItemSpawnRate = itemSpawnRate;
			FluidAmount = fluidAmount;
			CrateModifier = crateModifier;
		}
	}
}
