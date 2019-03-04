using UnityEngine;

internal class GuiUtilities
{
	public static string FormatTime(float seconds)
	{
		int num = Mathf.FloorToInt(seconds / 60f);
		int num2 = Mathf.FloorToInt(seconds - (float)(num * 60));
		return num.ToString() + ":" + num2.ToString("D2");
	}

	public static string FormatTimeHMMSS(int seconds)
	{
		int num = Mathf.FloorToInt((float)seconds / 60f);
		seconds = Mathf.FloorToInt((float)(seconds - num * 60));
		int num2 = Mathf.FloorToInt((float)num / 60f);
		num = Mathf.FloorToInt((float)(num - num2 * 60));
		return num2.ToString() + ":" + num.ToString("D2") + ":" + seconds.ToString("D2");
	}
}
