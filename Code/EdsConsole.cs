using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class EdsConsole
{
	private const int lines = 20;

	private static List<string> data = new List<string>();

	public static ReadOnlyCollection<string> GetData()
	{
		return data.AsReadOnly();
	}

	public static void Print(string line)
	{
		data.Add(line);
		if (data.Count > 20)
		{
			data.RemoveRange(0, data.Count - 20);
		}
	}
}
