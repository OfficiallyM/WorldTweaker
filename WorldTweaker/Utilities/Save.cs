using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldTweaker.Core;

namespace WorldTweaker.Utilities
{
	internal static class Save
	{
		private static SaveData _data;

		private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		};

		// Queue variables.
		private static float _lastQueueRunTime = 0;
		private static List<QueueEntry> _queue = new List<QueueEntry>();
		private static readonly int _queueInterval = 2;
		private static bool _isQueueRunning = false;

		/// <summary>
		/// Read/write data to game save.
		/// <para>Originally from RundensWheelPositionEditor</para>
		/// </summary>
		/// <param name="input">The string to write to the save</param>
		/// <returns>The read/written string</returns>
		private static string ReadWriteData(string input = null)
		{
			try
			{
				save_rendszam saveRendszam = null;
				save_prefab savePrefab1;

				// Attempt to find existing plate.
				if ((savedatascript.d.data.farStuff.TryGetValue(Mathf.Abs(WorldTweaker.I.ID.GetHashCode()), out savePrefab1) || savedatascript.d.data.nearStuff.TryGetValue(Mathf.Abs(WorldTweaker.I.ID.GetHashCode()), out savePrefab1)) && savePrefab1.rendszam != null)
					saveRendszam = savePrefab1.rendszam;

				// Plate doesn't exist.
				if (saveRendszam == null)
				{
					// Create a new plate to store the input string in.
					tosaveitemscript component = itemdatabase.d.gplate.GetComponent<tosaveitemscript>();
					save_prefab savePrefab2 = new save_prefab(component.category, component.id, double.MaxValue, double.MaxValue, double.MaxValue, 0.0f, 0.0f, 0.0f);
					savePrefab2.rendszam = new save_rendszam();
					saveRendszam = savePrefab2.rendszam;
					saveRendszam.S = string.Empty;
					savedatascript.d.data.farStuff.Add(Mathf.Abs(WorldTweaker.I.ID.GetHashCode()), savePrefab2);
				}

				// Write the input to the plate.
				if (input != null && input != string.Empty)
					saveRendszam.S = input;

				return saveRendszam.S;
			}
			catch (Exception ex)
			{
				Logging.LogError($"Save ReadWriteData() error. Details: {ex}");
			}

			return string.Empty;
		}

		/// <summary>
		/// Unserialize existing save data.
		/// </summary>
		/// <returns>Unserialized save data</returns>
		private static SaveData Get()
		{
			if (_data != null) return _data;

			try
			{
				string existingString = ReadWriteData();
				if (string.IsNullOrEmpty(existingString)) return null;
				_data = JsonConvert.DeserializeObject<SaveData>(existingString, _jsonSettings);
				return _data;
			}
			catch (Exception ex)
			{
				Logging.LogError($"Save Get() error. Details: {ex}");
			}

			return null;
		}

		/// <summary>
		/// Serialize save data and write to save.
		/// </summary>
		/// <param name="data">The data to serialize</param>
		private static void Set(SaveData data)
		{
			try
			{
				_data = data;
				ReadWriteData(JsonConvert.SerializeObject(_data, Formatting.None, _jsonSettings));
			}
			catch (Exception ex)
			{
				Logging.LogError($"Save Set() error. Details: {ex}");
			}
		}

		/// <summary>
		/// Update if already exists, otherwise insert new save data.
		/// </summary>
		/// <param name="data">Data to insert</param>
		public static void Upsert(Savable data)
		{
			QueueEntry entry = _queue.FirstOrDefault(q => q.Data.Type == data.Type);
			if (entry != null)
				entry.Data = data;
			else
				_queue.Add(new QueueEntry() { Data = data });
		}

		/// <summary>
		/// Delete save data.
		/// </summary>
		/// <param name="id">Id of data to delete</param>
		/// <param name="type">Data type of data to delete</param>
		public static void Delete(string type)
		{
			QueueEntry entry = _queue.FirstOrDefault(q => q.Data.Type == type);
			if (entry != null)
				entry.QueueType = QueueType.delete;
			else
				_queue.Add(new QueueEntry() { QueueType = QueueType.delete, Data = new ToDelete() { Type = type } });
		}

		/// <summary>
		/// Get save data by id and type.
		/// </summary>
		/// <param name="id">Id to search</param>
		/// <param name="type">Data type</param>
		/// <returns>Save data if exists, otherwise null</returns>
		private static Savable GetData(string type)
		{
			return Get()?.Data?.FirstOrDefault(d => d.Type == type);
		}

		/// <summary>
		/// Get world data.
		/// </summary>
		/// <returns>World data if exists, otherwise null</returns>
		public static WorldData GetWorldData()
		{
			return (WorldData)GetData(nameof(WorldData));
		}

		/// <summary>
		/// Trigger save data queue execution.
		/// </summary>
		internal static void ExecuteQueue()
		{
			int currentQueueInterval = Mathf.RoundToInt(Time.unscaledTime - _lastQueueRunTime);
			if (currentQueueInterval < _queueInterval)
				return;

			if (_queue.Count > 0 && !_isQueueRunning)
			{
				_isQueueRunning = true;
				int upserts = _queue.Where(q => q.QueueType == QueueType.upsert).ToList().Count;
				int deletes = _queue.Where(q => q.QueueType == QueueType.delete).ToList().Count;

				SaveData data = Get();
				if (data == null)
					data = new SaveData();
				if (data.Data == null)
					data.Data = new List<Savable>();

				foreach (QueueEntry entry in _queue)
				{
					switch (entry.QueueType)
					{
						case QueueType.upsert:
							Savable existing = data.Data.FirstOrDefault(d => d.Type == entry.Data.Type);
							if (existing != null)
							{
								int index = data.Data.IndexOf(existing);
								existing = entry.Data;
								data.Data[index] = existing;
							}
							else
								data.Data.Add(entry.Data);
							break;
						case QueueType.delete:
							Savable save = data.Data.FirstOrDefault(d => d.Type == entry.Data.Type);
							if (save != null)
								data.Data.Remove(save);
							break;
					}
				}

				Set(data);
				_queue.Clear();
				_isQueueRunning = false;
			}

			_lastQueueRunTime = Time.unscaledTime;
		}
	}
}
