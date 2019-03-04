using Svelto.DataStructures;
using System.Collections.Generic;

internal class CubeListData
{
	public uint cpuRating
	{
		get;
		private set;
	}

	public int health
	{
		get;
		private set;
	}

	public float healthBoost
	{
		get;
		private set;
	}

	public ItemCategory itemCategory
	{
		get;
		private set;
	}

	public uint placementFacesMask
	{
		get;
		private set;
	}

	public bool isIndestructible
	{
		get;
		private set;
	}

	public bool protoniumCube
	{
		get;
		private set;
	}

	public string descriptionStrKey
	{
		get;
		private set;
	}

	public ReadOnlyDictionary<string, object> displayStats
	{
		get;
		private set;
	}

	public SpecialCubesKind specialCubeKind
	{
		get;
		private set;
	}

	public ItemDescriptor itemDescriptor
	{
		get;
		private set;
	}

	public ItemSize itemSize
	{
		get;
		private set;
	}

	public int leagueUnlockIndex
	{
		get;
		private set;
	}

	public BuildVisibility buildVisibility
	{
		get;
		private set;
	}

	public bool greyOutInTutorial
	{
		get;
		private set;
	}

	public ItemType itemType
	{
		get;
		private set;
	}

	public int robotRanking
	{
		get;
		private set;
	}

	public bool isCosmetic
	{
		get;
		private set;
	}

	public CubeTypeID variantOf
	{
		get;
		private set;
	}

	public CubeListData(uint cpu, int h, float healthBoost_, ItemCategory cat, uint placements, bool protonium, Dictionary<string, object> displayStats_, string descriptionStrKey_, SpecialCubesKind specialCubeKind_, ItemSize itemSize_, int leagueUnlockIndex_, bool isIndestructible_, BuildVisibility buildVisibility_, bool greyOutInTutorial_, ItemType itemType_, int robotRanking_, bool isCosmetic_, CubeTypeID variantOf_)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		cpuRating = cpu;
		health = h;
		healthBoost = healthBoost_;
		itemCategory = cat;
		placementFacesMask = placements;
		isIndestructible = isIndestructible_;
		protoniumCube = protonium;
		leagueUnlockIndex = leagueUnlockIndex_;
		robotRanking = robotRanking_;
		isCosmetic = isCosmetic_;
		buildVisibility = buildVisibility_;
		greyOutInTutorial = greyOutInTutorial_;
		itemType = itemType_;
		variantOf = variantOf_;
		if (displayStats_ != null)
		{
			displayStats = new ReadOnlyDictionary<string, object>(displayStats_);
		}
		descriptionStrKey = descriptionStrKey_;
		specialCubeKind = specialCubeKind_;
		itemSize = itemSize_;
		switch (itemType)
		{
		case ItemType.Module:
			itemDescriptor = new ModuleDescriptor(itemCategory, itemSize);
			break;
		case ItemType.Weapon:
			if (itemCategory == ItemCategory.Chaingun)
			{
				itemDescriptor = new ChaingunDescriptor(itemCategory, itemSize);
			}
			else
			{
				itemDescriptor = new WeaponDescriptor(itemCategory, itemSize);
			}
			break;
		case ItemType.Movement:
			if (itemCategory == ItemCategory.Wing || itemCategory == ItemCategory.Rudder)
			{
				itemDescriptor = new AerofoilDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Wheel)
			{
				itemDescriptor = new WheelDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Ski)
			{
				itemDescriptor = new SkiDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Hover)
			{
				itemDescriptor = new HoverDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.TankTrack)
			{
				itemDescriptor = new TankDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Rotor)
			{
				itemDescriptor = new RotorDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Propeller)
			{
				itemDescriptor = new PropellerDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.Thruster)
			{
				itemDescriptor = new ThrusterDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.MechLeg || itemCategory == ItemCategory.SprinterLeg)
			{
				itemDescriptor = new MechLegDescriptor(itemCategory, itemSize);
			}
			else if (itemCategory == ItemCategory.InsectLeg)
			{
				itemDescriptor = new InsectLegDescriptor(itemCategory, itemSize);
			}
			else
			{
				itemDescriptor = new MovementDescriptor(itemCategory, itemSize);
			}
			break;
		case ItemType.Cosmetic:
			itemDescriptor = new CosmeticDescriptor(itemCategory, itemSize);
			break;
		}
	}
}
