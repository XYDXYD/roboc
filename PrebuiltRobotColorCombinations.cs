using RoboCraft.MiniJSON;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;

internal class PrebuiltRobotColorCombinations
{
	public FasterList<byte[]> colors
	{
		get;
		private set;
	}

	public PrebuiltRobotColorCombinations(string responseString)
	{
		List<object> list = (List<object>)Json.Deserialize(responseString);
		colors = new FasterList<byte[]>();
		for (int i = 0; i < list.Count; i++)
		{
			List<object> list2 = (List<object>)list[i];
			byte[] array = new byte[list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				array[j] = Convert.ToByte(list2[j]);
			}
			colors.Add(array);
		}
	}
}
