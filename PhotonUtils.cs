using System;
using System.Collections.Generic;

internal static class PhotonUtils
{
	public static void AddParameter<T>(this Dictionary<byte, object> parameters, Enum code, T value)
	{
		parameters.Add(Convert.ToByte(code), value);
	}

	public static T GetParameter<T>(this Dictionary<byte, object> parameters, Enum code)
	{
		return (T)parameters[Convert.ToByte(code)];
	}
}
