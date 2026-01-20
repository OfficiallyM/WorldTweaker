using System.Collections.Generic;
using System.Linq;
using TLDLoader;
using UnityEngine;
using WorldTweaker.Core;
using WorldTweaker.Utilities;

namespace WorldTweaker
{
	public class WorldTweaker : Mod
	{
		// Mod meta stuff.
		public override string ID => "M_WorldTweaker";
		public override string Name => "World Tweaker";
		public override string Author => "M-";
		public override string Version => "1.0.0";
		public override bool LoadInMenu => true;
		public override bool UseLogger => true;
		public override bool UseHarmony => true;

		internal static WorldTweaker I;

		internal float RoadLength = 5000000;
		private Dictionary<float, string> _lengths = new Dictionary<float, string>()
		{   
			{ 100, "Ridiculously short" },
			{ 500000, "Very short (500 km)" },
			{ 1000000, "Short (1,000 km)" },
			{ 2500000, "Medium (2,500 km)" },
			{ 5000000, "Vanilla (5,000 km)" },
			{ 10000000, "Long (10,000 km)" },
			{ 20000000, "Very long (20,000 km)" },
		};
		private List<float> _lengthKeys;
		private int _selectedLength = 4;

		public WorldTweaker()
		{
			I = this;

			_lengthKeys = _lengths.Keys.OrderBy(v => v).ToList();
		}

		public override void OnMenuLoad()
		{
			Logging.Log("OnMenuLoad()");
		}

		public override void OnLoad()
		{
			// Don't save data for loaded saves.
			if (mainscript.M.load) return;

			Save.Upsert(new RoadData(RoadLength));
		}

		public override void Update()
		{
			Save.ExecuteQueue();
		}

		public override void OnGUI()
		{
			if (ModLoader.isOnMainMenu)
			{
				// Don't render the UI if any game menus are open.
				if (mainmenuscript.mainmenu.SettingsScreenObj.activeSelf || mainmenuscript.mainmenu.SaveScreenObj.activeSelf) return;

				float width = 300f;
				float height = 300f;
				float x = (Screen.width / 2) - (width / 2);
				float y = (Screen.height / 2) - (height / 2);
				GUILayout.BeginArea(new Rect(x, y, width, height), $"<size=16><b>World settings</b></size>", "box");
				GUILayout.BeginVertical();
				GUILayout.Space(10);
				GUILayout.Label("Road length");
				_selectedLength = Mathf.RoundToInt(GUILayout.HorizontalSlider(_selectedLength, 0, _lengthKeys.Count - 1));
				RoadLength = _lengthKeys[_selectedLength];
				GUILayout.Label(_lengths[RoadLength]);
				GUILayout.EndVertical();
				GUILayout.EndArea();
				return;
			}
		}
	}
}
