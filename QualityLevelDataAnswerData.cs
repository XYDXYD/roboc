using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;

internal class QualityLevelDataAnswerData
{
	public const string QUALITY_LEVEL_OBJ = "Level";

	public const string DEFAULT_OBJ = "default";

	public readonly List<AutoQualityChooser.QualityLevelData> qualityLevels;

	public int lowMemoryThreshold;

	public int extermeLowMemoryThreshold;

	public QualityLevelDataAnswerData(Dictionary<string, Hashtable> data)
	{
		qualityLevels = new List<AutoQualityChooser.QualityLevelData>();
		Hashtable val = data["qualityLevels"];
		foreach (DictionaryEntry item in val)
		{
			AutoQualityChooser.QualityLevelData qualityLevelData = new AutoQualityChooser.QualityLevelData();
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			qualityLevelData.level = (int)(long)dictionary["Level"];
			qualityLevelData.levelMults = Convert.ToSingle(dictionary["default"]);
			qualityLevels.Add(qualityLevelData);
		}
		Hashtable val2 = data["systemMemoryThresholds"];
		lowMemoryThreshold = Convert.ToInt32(val2.get_Item((object)"low"));
		extermeLowMemoryThreshold = Convert.ToInt32(val2.get_Item((object)"extremeLow"));
	}
}
