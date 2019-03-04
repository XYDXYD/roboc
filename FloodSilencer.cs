using System.Collections.Generic;
using UnityEngine;

internal static class FloodSilencer
{
	private const int _messageLimit = 4;

	private const float _floodInterval = 4f;

	private const float _silenceInterval = 10f;

	private static List<float> _messageTimeStamps = new List<float>();

	private static int _silenceCount = 0;

	private static float _silenceTime = 0f;

	private static bool _wasSilenced = false;

	public static bool ValidateFloodDetection()
	{
		if (CurrentlySilenced())
		{
			return false;
		}
		if (_wasSilenced)
		{
			_messageTimeStamps.Clear();
			_wasSilenced = false;
		}
		if (FloodTriggered())
		{
			ActivateSilence();
			return false;
		}
		return true;
	}

	private static void ActivateSilence()
	{
		_silenceCount++;
		_silenceTime = Time.get_realtimeSinceStartup();
		_wasSilenced = true;
	}

	private static bool CurrentlySilenced()
	{
		return Time.get_realtimeSinceStartup() - _silenceTime < 10f * (float)_silenceCount;
	}

	private static bool FloodTriggered()
	{
		bool result = false;
		_messageTimeStamps.Add(Time.get_realtimeSinceStartup());
		if (_messageTimeStamps.Count >= 4)
		{
			if (_messageTimeStamps[3] - _messageTimeStamps[0] <= 4f)
			{
				result = true;
			}
			_messageTimeStamps.RemoveAt(0);
		}
		return result;
	}
}
