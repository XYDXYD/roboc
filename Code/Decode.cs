using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

public static class Decode
{
	public static T Get<T>(IDictionary d, object key)
	{
		if (d.Contains(key))
		{
			object data = d[key];
			return Cast<T>(data, key);
		}
		string text = "The key '" + key + "' was not found";
		Console.LogError(text);
		throw new KeyNotFoundException(text);
	}

	public static T GetOptional<T>(Dictionary<string, object> d, string key, T defval = default(T))
	{
		object value;
		return (!d.TryGetValue(key, out value)) ? defval : Cast<T>(value, key);
	}

	public static T GetOptional<T>(Hashtable d, string key, T defval = default(T))
	{
		return (!d.ContainsKey(key)) ? defval : Cast<T>(d[key], key);
	}

	public static T Cast<T>(object data, object key)
	{
		try
		{
			return (T)Convert.ChangeType(data, typeof(T));
		}
		catch (InvalidCastException)
		{
			Console.LogError("Expected to find a '" + typeof(T) + "' in field '" + key + "', found '" + data + "' instead!");
			throw;
		}
	}
}
