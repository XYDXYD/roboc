using ExitGames.Client.Photon;
using Svelto.DataStructures;
using System.Collections.Generic;

public class PrebuiltRobotsDependency
{
	public Dictionary<string, FasterList<RobotPartData>> prebuiltRobotsByClass = new Dictionary<string, FasterList<RobotPartData>>();

	public Dictionary<string, RobotPartData> prebuiltRobotsById = new Dictionary<string, RobotPartData>();

	public PrebuiltRobotsDependency(Dictionary<string, Hashtable> jsonData)
	{
		foreach (KeyValuePair<string, Hashtable> jsonDatum in jsonData)
		{
			string key = jsonDatum.Key;
			string nameStrKey = (string)jsonDatum.Value.get_Item((object)"Name");
			string text = (string)jsonDatum.Value.get_Item((object)"Class");
			string categoryStrKey = (string)jsonDatum.Value.get_Item((object)"Category");
			byte[] data = (byte[])jsonDatum.Value.get_Item((object)"RobotData");
			byte[] colourData = (byte[])jsonDatum.Value.get_Item((object)"ColourData");
			RobotPartData robotPartData = new RobotPartData(key, nameStrKey, text, categoryStrKey, data, colourData);
			if (!prebuiltRobotsByClass.ContainsKey(text))
			{
				prebuiltRobotsByClass.Add(text, new FasterList<RobotPartData>());
			}
			prebuiltRobotsByClass[text].Add(robotPartData);
			prebuiltRobotsById.Add(key, robotPartData);
		}
	}
}
