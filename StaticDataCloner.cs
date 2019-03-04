using System;
using System.IO;

internal static class StaticDataCloner
{
	private static int? _hostPort;

	public static void Init(int? hostPort)
	{
		_hostPort = hostPort;
		Directory.CreateDirectory("JsonFiles");
		Directory.CreateDirectory("JsonFiles\\" + _hostPort);
	}

	public static void CopyJsonValue(string fileName, string data)
	{
		using (StreamWriter streamWriter = File.AppendText("JsonFiles\\" + _hostPort + "\\" + fileName))
		{
			streamWriter.WriteLine(DateTime.UtcNow + " " + data);
		}
	}
}
