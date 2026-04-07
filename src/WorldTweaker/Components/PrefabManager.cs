using System.Collections.Generic;
using System.Linq;
using TLDLoader.Extensions;
using UnityEngine;
using WorldTweaker.Utilities;

namespace WorldTweaker.Components
{
	internal class PrefabManager : MonoBehaviour
	{
		public GameObject Water;
		public GameObject Lava;
		public Shader WaterShader;
		public Texture WaterTexture;
		public Material LavaMaterial;
		public GameObject[] PalmTrees;
		public AudioClip Burn;
		public GameObject Coconut;
		public Sprite CoconutInv;

		public void CreatePrefabs()
		{
			// Water.
			Water = new GameObject("Water");
			Water.transform.position = Vector3.negativeInfinity;
			var originShift = Water.AddComponent<visszarako>();
			originShift.moveWhenParented = true;
			Water.AddComponent<MeshFilter>().mesh = itemdatabase.d.gerror.GetComponentInChildren<MeshFilter>().mesh;
			var renderer = Water.AddComponent<MeshRenderer>();
			var material = new Material(WaterShader);
			material.mainTexture = WaterTexture;
			material.color = new Color(0.6f, 0.9f, 1f, 0.75f);
			renderer.material = material;
			Water.AddComponent<Water>().material = material;

			// Lava.
			Lava = new GameObject("Lava");
			Lava.transform.position = Vector3.negativeInfinity;
			originShift = Lava.AddComponent<visszarako>();
			originShift.moveWhenParented = true;
			Lava.AddComponent<MeshFilter>().mesh = itemdatabase.d.gerror.GetComponentInChildren<MeshFilter>().mesh;
			renderer = Lava.AddComponent<MeshRenderer>();
			var lavaMaterial = new Material(LavaMaterial);
			lavaMaterial.mainTextureScale = new Vector2(200f, 200f);
			renderer.material = lavaMaterial;
			Lava.AddComponent<Lava>().material = lavaMaterial;

			// Coconuts.
			Coconut.AddComponent<Coconut>();
			SetupObject(Coconut, 0, invImg: CoconutInv);
			var coconutToSave = Coconut.GetComponent<tosaveitemscript>();
			var coconutTank = Coconut.CopyComponent(itemdatabase.d.gcactigib01.GetComponent<tankscript>());
			coconutTank.tosaveid = coconutToSave.idInSave;
			coconutTank.F.ChangeOne(0.5f, mainscript.fluidenum.water);
			coconutToSave.tanks = new tankscript[] { coconutTank };
			var coconutPickup = Coconut.GetComponent<pickupable>();
			coconutPickup.tank = coconutTank;
		}

		public tosaveitemscript MakeSavable(GameObject obj, int id)
		{
			if (obj.GetComponent<tosaveitemscript>() != null)
				return null;

			var toSave = obj.AddComponent<tosaveitemscript>();
			toSave.category = WorldTweaker.Category;
			toSave.id = id;
			toSave.RB = obj.GetComponent<Rigidbody>();
			toSave.randomizetanks = new tankscript[0];
			toSave.partconditions = new partconditionscript[0];
			toSave.tanks = new tankscript[0];
			toSave.tankcapscripts = new tankcapscript[0];
			toSave.partslotscripts = new partslotscript[0];
			toSave.usables = new usablescript[0];
			toSave.attachTargets = new attachTargetscript[0];
			toSave.door_rots = new door_rot[0];
			toSave.randomScales = new randomScaleScript[0];
			toSave.physlocks = new mountStuff[0];
			toSave.mirrorsStart = new List<mirrorscript>();
			toSave.interiorLights = new List<interiorLightScript>();

			var attach = obj.GetComponent<attachablescript>();
			if (attach != null)
				toSave.attachable = attach;

			return toSave;
		}

		private void SetupObject(GameObject obj, int id, bool pickupable = true, bool attachable = true, Sprite invImg = null)
		{
			var toSave = MakeSavable(obj, 0);

			obj.CopyComponent(itemdatabase.d.gww2compass.GetComponent<visszarako>());

			var rb = obj.GetComponent<Rigidbody>();
			if (rb == null)
				return;

			var mass = obj.AddComponent<massScript>();
			mass.SetMass(rb.mass);

			if (!pickupable)
				return;

			var pickup = obj.CopyComponent(itemdatabase.d.gww2compass.GetComponent<pickupable>());
			pickup.mass = mass;
			mass.P = pickup;
			pickup.RB = rb;
			pickup.invImg = invImg;

			List<layerScript> layers = new List<layerScript>();
			var colliders = obj.GetComponentsInChildren<Collider>().ToList();
			foreach (var collider in colliders)
			{
				var layer = collider.gameObject.AddComponent<layerScript>();
				layer.col = collider;
				layer.p = pickup;
				layers.Add(layer);
			}
			pickup.cols = layers.ToArray();

			if (!attachable)
				return;

			var attach = obj.CopyComponent(itemdatabase.d.gww2compass.GetComponent<attachablescript>());
			attach.C = colliders;
			pickup.attach = attach;
			toSave.attachable = attach;
		}
	}
}
