using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TLDLoader;
using UnityEngine;
using WorldTweaker.Components;
using WorldTweaker.Core;
using WorldTweaker.Harmony;
using WorldTweaker.UI;
using WorldTweaker.Utilities;
using WorldTweaker.Utilities.UI;

namespace WorldTweaker
{
	public class WorldTweaker : Mod
	{
		// Mod meta stuff.
		private string _version = "2.0.0";
		public override string ID => "M_WorldTweaker";
		public override string Name => "World Tweaker";
		public override string Author => "M-";
		public override string Version => _version;
		public override bool LoadInMenu => true;
		public override bool LoadInDB => true;
		public override bool UseAssetsFolder => true;
		public override bool UseLogger => true;
		public override bool UseHarmony => true;

		internal static WorldTweaker I;

		internal static bool IsOnMenu => mainscript.M == null;
		internal static bool IsPaused
		{
			get
			{
				var menu = mainscript.M?.menu?.Menu;
				return menu != null && menu.activeSelf;
			}
		}
		internal static bool Debug = false;
		internal static PrefabManager Prefabs;
		internal static WaterManager Water;

		internal bool InterceptStart = true;
		internal bool ShowUI = false;

		private bool _firstRun = true;
		private string[] _tabs = { "World", "Road", "Items" };
		private int _activeTab = 0;
		private int[] _gameTabs = { 2 };
		private bool _hasAchievementsMod = false;

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
				new OptionSlider<float>(0.33f, "1/3 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(0.75f, "3/4 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(1.5f, "1.5x vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla (painfully slow to load)"),
			},
			6
		);

		internal IndexSlider<float> MountainDensity = new IndexSlider<float>(
			"Mountain density (large rocks)",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.33f, "1/3 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(0.75f, "3/4 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(1.5f, "1.5x vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla (painfully slow to load)"),
			},
			6
		);

		internal IndexSlider<float> BuildingDensity = new IndexSlider<float>(
			"Building density",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.33f, "1/3 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(0.75f, "3/4 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(1.5f, "1.5x vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla"),
			},
			6
		);

		internal IndexSlider<float> WorldType = new IndexSlider<float>(
			"World type",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Flat"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(0.85f, "Canyon"),
				new OptionSlider<float>(1.25f, "Road bridge"),
				new OptionSlider<float>(2f, "Tropical"),
			},
			1
		);

		internal IndexSlider<float> ItemSpawnRate = new IndexSlider<float>(
			"Item spawn rate",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Zero"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.33f, "1/3 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(0.75f, "3/4 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(1.5f, "1.5x vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla"),
			},
			6
		);

		internal IndexSlider<float> FluidAmount = new IndexSlider<float>(
			"Fluid amount",
			"Controls how full randomised fluid containers are",
			new List<OptionSlider<float>>
			{
				new OptionSlider<float>(0f, "Always empty"),
				new OptionSlider<float>(0.1f, "1/10 vanilla"),
				new OptionSlider<float>(0.25f, "1/4 vanilla"),
				new OptionSlider<float>(0.33f, "1/3 vanilla"),
				new OptionSlider<float>(0.5f, "1/2 vanilla"),
				new OptionSlider<float>(0.75f, "3/4 vanilla"),
				new OptionSlider<float>(1f, "Vanilla"),
				new OptionSlider<float>(1.5f, "1.5x vanilla"),
				new OptionSlider<float>(2f, "2x vanilla"),
				new OptionSlider<float>(5f, "5x vanilla"),
				new OptionSlider<float>(10f, "10x vanilla"),
				new OptionSlider<float>(100f, "100x vanilla"),
			},
			6
		);

		internal IndexSlider<int> CrateModifier = new IndexSlider<int>(
			"Crate modifier",
			"Modifies how the contents of crates are handled",
			new List<OptionSlider<int>>
			{
				new OptionSlider<int>(0, "Always empty"),
				new OptionSlider<int>(1, "Vanilla"),
				new OptionSlider<int>(2, "Decrease amount, randomise condition"),
			},
			1
		);

		public WorldTweaker()
		{
			I = this;

#if DEBUG
			_version += "-DEV";
			Debug = true;
#endif
		}

		public override void OnMenuLoad()
		{
			_hasAchievementsMod = ModLoaderUtilities.DoesModExist("M_Achievements");

			// Initialise global helpers.
			GameObject helper = new GameObject("WorldTweaker");
			GameObject.DontDestroyOnLoad(helper);
			Prefabs = helper.AddComponent<PrefabManager>();
		}

		public override void DbLoad()
		{
			Save.InvalidateCache();
			Prefabs.CreatePrefabs();
			Water = mainscript.M.gameObject.AddComponent<WaterManager>();
		}

		public override void OnLoad()
		{
			// Don't save data for loaded saves.
			if (mainscript.M.load || (mainscript.M.DFMS?.load ?? false))
				return;

			UpdateSaveData();
		}

		public override void Update()
		{
			if (_firstRun)
			{
				HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony($"com.{ID.ToLower()}.2");
				if (ModLoaderUtilities.DoesModExist("SgtJoeBuildings"))
				{
					var buildingsMod = AppDomain.CurrentDomain.GetAssemblies()
						.FirstOrDefault(a => a.GetName().Name == "SgtJoeBuildings");

					if (buildingsMod != null)
					{
						var type = buildingsMod.GetType("SgtJoeBuildings.SgtJoeBuildingBase");

						// Get the display class type too for the signature
						var displayClassType = type.GetNestedType("<>c__DisplayClass32_0", BindingFlags.NonPublic);

						var method = type.GetMethod(
							"ParseItemFromSpawnString123123123123123", // replace with real name
							BindingFlags.NonPublic | BindingFlags.Static,   // note: static, not instance
							null,
							new Type[] { typeof(string), typeof(string).MakeByRefType(), typeof(int).MakeByRefType(), displayClassType.MakeByRefType() },
							null
						);

						harmonyInstance.Patch(method, postfix: new HarmonyMethod(typeof(Patch_SgtJoeBuildingBase_ParseItemFromSpawnString), nameof(Patch_SgtJoeBuildingBase_ParseItemFromSpawnString.Postfix)));
					}
				}
				_firstRun = false;
			}

			Save.ExecuteQueue();

			// Reset UI intercept here because OnMenuLoad() is only called once.
			if (IsOnMenu && !InterceptStart)
				InterceptStart = true;

			if (ShowUI && Input.GetButtonDown("Cancel"))
				ToggleUI(false);
		}

		internal void ToggleUI(bool? force = null, bool animate = true)
		{
			ShowUI = force ?? !ShowUI;
			SetStartButtonState(!ShowUI);

			if (!IsOnMenu)
			{
				if (ShowUI && IsPaused)
					mainscript.M.PressedEscape();

				mainscript.M.crsrLocked = !ShowUI;
				mainscript.M.SetCursorVisible(ShowUI);
				mainscript.M.menu.gameObject.SetActive(!ShowUI);
			}

			if (!animate)
			{
				Animator.Reset("mainUI");
				return;
			}

			if (ShowUI)
				Animator.Play("mainUI", Animator.AnimationState.SlideIn);
			else
				Animator.Play("mainUI", Animator.AnimationState.SlideOut);
		}

		public override void OnGUI()
		{
			Styling.Bootstrap();
			GUI.skin = Styling.GetSkin();

			Rect buttonRect = new Rect(Screen.width * 0.70f, _hasAchievementsMod ? 70f : 10f, 200, 50);
			if (!IsOnMenu && IsPaused && GUI.Button(buttonRect, "World Tweaker", "ButtonSecondary"))
				ToggleUI();

			if (ShowUI || !Animator.IsIdle("mainUI"))
				RenderMenu();

			// Reset back to default Unity skin to avoid styling bleeding to other UI mods.
			GUI.skin = null;
		}

		private void RenderMenu()
		{
			// Don't render the UI if any game menus are open.
			if (IsOnMenu && (mainmenuscript.mainmenu.SettingsScreenObj.activeSelf || mainmenuscript.mainmenu.SaveScreenObj.activeSelf)) 
				return;

			float width = Screen.width * 0.25f;
			float height = Screen.height * 0.75f;
			float x = Screen.width / 2 - width / 2;
			float y = Screen.height / 2 - height / 2;
			Rect targetRect = new Rect(x, y, width, height);
			Rect animatedRect = Animator.Slide("mainUI", targetRect, Animator.SlideDirection.Bottom);

			if (!ShowUI && Animator.IsIdle("mainUI"))
				return;

			GUILayout.BeginArea(animatedRect, $"<color=#f87ffa><size=18><b>{Name}</b></size>\n<size=16>Made with ❤️ by {Author}</size></color>", "box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
				ToggleUI(false);
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(15);
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			for (int i = 0; i < _tabs.Length; i++)
			{
				if (!IsOnMenu && Array.IndexOf(_gameTabs, i) == -1)
				{
					if (_activeTab == i)
						_activeTab = _gameTabs[0];
					continue;
				}

				if (GUILayout.Button(_tabs[i], _activeTab == i ? "ButtonSecondary" : "button"))
					_activeTab = i;
			}
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			switch (_activeTab)
			{
				case 0: RenderWorldMenu(); break;
				case 1: RenderRoadMenu(); break;
				case 2: RenderItemsMenu(); break;
			}

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (IsOnMenu && GUILayout.Button("Start", "ButtonPrimaryLarge", GUILayout.MaxWidth(200), GUILayout.Height(40)))
			{
				InterceptStart = false;
				ToggleUI(false, false);
				mainmenuscript.mainmenu.PressedLoadScene("SceneNewRandom");
			}
			if (!IsOnMenu && GUILayout.Button("Apply", "ButtonPrimaryLarge", GUILayout.MaxWidth(200), GUILayout.Height(40)))
			{
				UpdateSaveData();
				ToggleUI(false);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.Label($"<color=#f87ffa><size=16>v{Version}</size></color>", "LabelCenter");
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void RenderWorldMenu()
		{
			BuildingDensity.Render();
			ObjectDensity.Render();
			MountainDensity.Render();
			WorldType.Render();
		}

		private void RenderRoadMenu()
		{
			RoadLength.Render();
			RoadCurvature.Render();
		}

		private void RenderItemsMenu()
		{
			if (!IsOnMenu)
				GUILayout.Label("Note: Any changes made here will only apply to buildings/items not yet generated.", "LabelCenter");

			ItemSpawnRate.Render();
			FluidAmount.Render();
			CrateModifier.Render();
		}

		public void SetStartButtonState(bool state)
		{
			if (!IsOnMenu) return;

			mainmenuscript.mainmenu.Canvas.Find("GameObject/MainStuff/ButtonStart").gameObject.SetActive(state);
		}

		public void UpdateSaveData()
		{
			Save.Upsert(new WorldData(
				RoadLength.Value,
				RoadCurvature.Value,
				ObjectDensity.Value,
				MountainDensity.Value,
				BuildingDensity.Value,
				WorldType.Value,
				ItemSpawnRate.Value,
				FluidAmount.Value,
				CrateModifier.Value
			));
		}
	}
}
