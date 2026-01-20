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

	internal class RoadData : Savable
	{
		public float Length { get; set; }

		public RoadData(float length)
		{
			Length = length;
		}
	}
}
