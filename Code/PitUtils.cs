using System;

internal static class PitUtils
{
	private static uint MAXIMUM_STREAK_SCORE_LIMIT = 5u;

	private static uint LEADER_BONUS = 1u;

	public static uint GetPlayerValue(bool isLeader, uint currentStreak)
	{
		currentStreak = Math.Min(currentStreak, MAXIMUM_STREAK_SCORE_LIMIT);
		return 1 + ((currentStreak >= 2) ? (currentStreak - 1) : 0) + (isLeader ? LEADER_BONUS : 0);
	}

	internal static string TitleForStreak(uint streak)
	{
		switch (streak)
		{
		case 0u:
		case 1u:
			return "-";
		case 2u:
			return StringTableBase<StringTable>.Instance.GetString("strRampant");
		case 3u:
			return StringTableBase<StringTable>.Instance.GetString("strDominant");
		case 4u:
			return StringTableBase<StringTable>.Instance.GetString("strUnstoppable");
		default:
			return StringTableBase<StringTable>.Instance.GetString("strLegendary");
		}
	}
}
