using System;
using System.Collections;
using System.IO;
using System.Xml;
using Utility;

public static class ServEnv
{
	private const string FILE_NAME = "servenvmulti.config";

	public static bool Exists()
	{
		return File.Exists("servenvmulti.config");
	}

	private static XmlDocument Load()
	{
		if (!Exists())
		{
			Console.LogError("servenvmulti.config does not exist (to avoid getting this error check first with ServEnv.Exists()");
			return null;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load("servenvmulti.config");
		return xmlDocument;
	}

	public static bool TryGetValue(string key, out string value)
	{
		XmlDocument xmlDocument = Load();
		if (xmlDocument == null)
		{
			value = null;
			return false;
		}
		string innerText = xmlDocument.SelectSingleNode("servers/currentgroup").InnerText;
		string innerText2 = xmlDocument.SelectSingleNode("servers/currentsku").InnerText;
		XmlNodeList xmlNodeList = xmlDocument.SelectNodes("servers/" + innerText2 + "/" + innerText + "/setting");
		IEnumerator enumerator = xmlNodeList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				XmlNode xmlNode = (XmlNode)enumerator.Current;
				if (xmlNode.Attributes["name"].Value == key)
				{
					value = xmlNode.InnerText;
					return !string.IsNullOrEmpty(value);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		value = null;
		return false;
	}
}
