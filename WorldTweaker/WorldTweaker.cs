using System.Collections.Generic;
using System.Linq;
using TLDLoader;
using UnityEngine;
using UnityEngine.UI;
using WorldTweaker.Core;
using WorldTweaker.UI;
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

		internal bool InterceptStart = true;
		internal bool ShowUI = false;

		internal IndexSlider<float> RoadLength = new IndexSlider<float>(
			"Road length",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(100, "Ridiculously short"),
				new OptionSlider<float>(500000, "Very short (500 km)"),
				new OptionSlider<float>(1000000, "Short (1,000 km)"),
				new OptionSlider<float>(2500000, "Medium (2,500 km)"),
				new OptionSlider<float>(5000000, "Vanilla (5,000 km)"),
				new OptionSlider<float>(10000000, "Long (10,000 km)"),
				new OptionSlider<float>(20000000, "Very long (20,000 km)"),
			},
			4
		);

		internal IndexSlider<float> RoadCurvature = new IndexSlider<float>(
			"Road curviness",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Straight"),
				new OptionSlider<float>(1f, "Vanilla"),
			},
			1
		);

		internal IndexSlider<float> ObjectDensity = new IndexSlider<float>(
			"Object density (cacti, rocks, etc)",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla (painfully slow to load)"),
			},
			4
		);

		internal IndexSlider<float> MountainDensity = new IndexSlider<float>(
			"Mountain density (large rocks)",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla (painfully slow to load)"),
			},
			4
		);

		internal IndexSlider<float> BuildingDensity = new IndexSlider<float>(
			"Building density",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla"),
			},
			4
		);

		public WorldTweaker()
		{
			I = this;
		}

		public override void OnMenuLoad()
		{
			Logging.LogDebug("OnMenuLoad()");
		}

		public override void OnLoad()
		{
			// Don't save data for loaded saves.
			if (mainscript.M.load) return;

			Save.Upsert(new WorldData(RoadLength.Value, ObjectDensity.Value, MountainDensity.Value, BuildingDensity.Value));
		}

		public override void Update()
		{
			Save.ExecuteQueue();

			// Reset UI intercept here because OnMenuLoad() is only called once.
			if (ModLoader.isOnMainMenu && !InterceptStart)
				InterceptStart = true;
		}

		public override void OnGUI()
		{
			if (ModLoader.isOnMainMenu)
			{
				if (!ShowUI) return;

				// Don't render the UI if any game menus are open.
				if (mainmenuscript.mainmenu.SettingsScreenObj.activeSelf || mainmenuscript.mainmenu.SaveScreenObj.activeSelf) return;

				float width = 300f;
				float height = 450f;
				float x = (Screen.width / 2) - (width / 2);
				float y = (Screen.height / 2) - (height / 2);
				GUILayout.BeginArea(new Rect(x, y, width, height), $"<size=16><b>World settings</b></size>", "box");
				GUILayout.BeginVertical();
				GUILayout.Space(20);

				RoadLength.Render();
				RoadCurvature.Render();
				BuildingDensity.Render();
				ObjectDensity.Render();
				MountainDensity.Render();

				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Start", GUILayout.MaxWidth(200), GUILayout.Height(30)))
				{
					InterceptStart = false;
					ShowUI = false;
					SetStartButtonState(true);
					mainmenuscript.mainmenu.PressedLoadScene("SceneNewRandom");
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
				GUILayout.EndArea();

				if (GUI.Button(new Rect(x + (width - 25f), y, 25f, 25f), "X"))
				{
					ShowUI = false;
					SetStartButtonState(true);
				}
				return;
			}
		}

		public void SetStartButtonState(bool state)
		{
			mainmenuscript.mainmenu.Canvas.Find("GameObject/MainStuff/ButtonStart").gameObject.SetActive(state);
		}
	}
}
