using ExitGames.Client.Photon;
using System;

public class CustomisationsEntry
{
	public enum CustomisationCategory
	{
		Skin,
		SpawnEffect,
		DeathEffect
	}

	public CustomisationCategory category
	{
		get;
		private set;
	}

	public string id
	{
		get;
		private set;
	}

	public string localisedName
	{
		get;
		private set;
	}

	public string skinSceneName
	{
		get;
		private set;
	}

	public string simulationPrefab
	{
		get;
		private set;
	}

	public string previewImageName
	{
		get;
		private set;
	}

	public bool alwaysUnlocked
	{
		get;
		private set;
	}

	public bool isDefault
	{
		get;
		private set;
	}

	public CustomisationsEntry(CustomisationCategory category_, string id_, string localisedName_, string skinSceneName_, string simulationPrefab_, string previewImageName_, bool alwaysUnlocked_, bool isDefault_)
	{
		category = category_;
		id = id_;
		localisedName = localisedName_;
		skinSceneName = skinSceneName_;
		simulationPrefab = simulationPrefab_;
		previewImageName = previewImageName_;
		alwaysUnlocked = alwaysUnlocked_;
		isDefault = isDefault_;
	}

	public static CustomisationsEntry DeserialiseFromHashtable(Hashtable table)
	{
		CustomisationCategory category_ = (CustomisationCategory)Enum.Parse(typeof(CustomisationCategory), Convert.ToString(table.get_Item((object)"category")), ignoreCase: true);
		string id_ = Convert.ToString(table.get_Item((object)"id"));
		string localisedName_ = Convert.ToString(table.get_Item((object)"localisedName"));
		string skinSceneName_ = Convert.ToString(table.get_Item((object)"skinsceneName"));
		string simulationPrefab_ = Convert.ToString(table.get_Item((object)"simulationPrefab"));
		string previewImageName_ = Convert.ToString(table.get_Item((object)"previewImageName"));
		bool alwaysUnlocked_ = Convert.ToBoolean(table.get_Item((object)"alwaysUnlocked"));
		bool isDefault_ = Convert.ToBoolean(table.get_Item((object)"isDefault"));
		return new CustomisationsEntry(category_, id_, localisedName_, skinSceneName_, simulationPrefab_, previewImageName_, alwaysUnlocked_, isDefault_);
	}
}
