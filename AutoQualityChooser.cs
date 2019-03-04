using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AutoQualityChooser
{
	public class QualityLevelData
	{
		public int level;

		public float levelMults;
	}

	private const int DefaultQuality = 0;

	private List<float> _levelMult = new List<float>
	{
		5f,
		30f,
		80f,
		130f,
		200f,
		320f
	};

	private List<QualityLevelData> _qualityLevels;

	private int _lowMemoryThreshold;

	private int _extremeLowMemoryThreshold;

	internal AutoQualityChooser(QualityLevelDataAnswerData data)
	{
		_qualityLevels = data.qualityLevels;
		_lowMemoryThreshold = data.lowMemoryThreshold;
		_extremeLowMemoryThreshold = data.extermeLowMemoryThreshold;
	}

	internal void CalculateQualityLevels()
	{
		SetQualityLevelData(_qualityLevels);
		ActuallyCalculateQualityLevels();
	}

	private void SetQualityLevelData(List<QualityLevelData> qualityLevels)
	{
		qualityLevels.Sort((QualityLevelData a, QualityLevelData b) => a.level.CompareTo(b.level));
		_levelMult.Clear();
		for (int i = 0; i < qualityLevels.Count; i++)
		{
			_levelMult.Add(qualityLevels[i].levelMults);
		}
	}

	private void ActuallyCalculateQualityLevels()
	{
		int qualityLevel = AutoChooseQuality();
		if (!PlayerPrefs.HasKey("QualityAutoCalculated"))
		{
			if (PlayerPrefs.GetInt("UnityGraphicsQuality") == 0)
			{
				Console.Log("Auto-calculating quality for first-time user");
				QualitySettings.SetQualityLevel(qualityLevel);
			}
			PlayerPrefs.SetInt("QualityAutoCalculated", 0);
		}
		PrintCalculatedQualityToLog(qualityLevel);
		PrintActualQualityToLog(QualitySettings.GetQualityLevel());
	}

	private int AutoChooseQuality()
	{
		int systemMemorySize = SystemInfo.get_systemMemorySize();
		int num;
		if (systemMemorySize < _extremeLowMemoryThreshold)
		{
			num = 0;
			ReportLowRam(systemMemorySize, num);
		}
		else if (systemMemorySize < _lowMemoryThreshold)
		{
			num = 1;
			ReportLowRam(systemMemorySize, num);
		}
		else
		{
			float fillrate = CalculateFillRate();
			float fillneed = (float)(Screen.get_width() * Screen.get_height() + 120000) * 6E-05f;
			num = CalculateQualityLevel(fillrate, fillneed);
		}
		return num;
	}

	private static void ReportLowRam(int memory, int quality)
	{
		Console.LogWarning($"Low system memory detected ({memory}), setting quality to {QualityLevelAsString(quality)}");
	}

	private static float CalculateFillRate()
	{
		float num = 0f;
		float num2 = SystemInfo.get_graphicsShaderLevel();
		float num3 = SystemInfo.get_graphicsMemorySize();
		float num4 = SystemInfo.get_processorCount();
		num = ((num2 < 10f) ? 1000f : ((num2 < 20f) ? 1300f : ((!(num2 < 30f)) ? 3000f : 2000f)));
		if (num4 >= 6f)
		{
			num *= 3f;
		}
		else if (num4 >= 3f)
		{
			num *= 2f;
		}
		if (num3 >= 512f)
		{
			num *= 2f;
		}
		else if (num3 <= 128f)
		{
			num /= 2f;
		}
		return num;
	}

	private int CalculateQualityLevel(float fillrate, float fillneed)
	{
		int i;
		for (i = 0; i < QualitySettings.get_names().Length - 1 && fillrate > fillneed * _levelMult[i + 1]; i++)
		{
		}
		return i;
	}

	private static void PrintActualQualityToLog(int qualityLevel)
	{
		string str = QualityLevelAsString(qualityLevel);
		Console.Log("Current quality on start: " + str);
	}

	private static void PrintCalculatedQualityToLog(int qualityLevel)
	{
		string text = qualityLevel.ToString();
		if (qualityLevel >= 0 && qualityLevel < QualitySettings.get_names().Length)
		{
			text = text + " (" + QualitySettings.get_names()[qualityLevel] + ")";
		}
		Console.Log("Auto calculated quality on start: " + text);
	}

	private static string QualityLevelAsString(int qualityLevel)
	{
		string text = qualityLevel.ToString();
		if (qualityLevel >= 0 && qualityLevel < QualitySettings.get_names().Length)
		{
			text = text + " (" + QualitySettings.get_names()[qualityLevel] + ")";
		}
		return text;
	}
}
