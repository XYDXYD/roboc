using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class ProfilerWrapper
{
	private class Sample
	{
		public string name;

		public int refcount;
	}

	private static Dictionary<string, Sample> _callerToSample = new Dictionary<string, Sample>();

	[Conditional("DEBUG")]
	public static void BeginSample(string name, Object targetObject)
	{
	}

	[Conditional("DEBUG")]
	public static void BeginSample(string name)
	{
	}

	[Conditional("DEBUG")]
	public static void EndSample()
	{
	}

	private static string GetCaller(StackFrame callerFrame)
	{
		return callerFrame.GetMethod().ReflectedType.Name + ":" + callerFrame.GetMethod().Name;
	}

	private static Sample GetOrCreateSample(string caller)
	{
		if (!_callerToSample.TryGetValue(caller, out Sample value))
		{
			value = new Sample();
			_callerToSample.Add(caller, value);
		}
		return value;
	}
}
