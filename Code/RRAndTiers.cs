using System;

internal static class RRAndTiers
{
	internal static string ConvertRobotRankingToTierString(uint totalRobotRanking, bool isMegabot, TiersData tiersData)
	{
		uint tierIndex = ConvertRRToTierIndex(totalRobotRanking, isMegabot, tiersData);
		return ConvertTierIndexToTierString(tierIndex, tiersData);
	}

	internal static string ConvertTierIndexToStringNoMegabotCheck(uint tierIndex)
	{
		string str = "T ";
		return str + (tierIndex + 1).ToString();
	}

	internal static bool IsMegabotTier(uint tierIndex, TiersData tiersData)
	{
		return tierIndex == tiersData.MegabotTierIndex;
	}

	internal static uint ConvertRRToTierIndex(uint totalRobotRanking, bool isMegabot, TiersData tiersData)
	{
		uint[] tiersBands = tiersData.TiersBands;
		int num = tiersBands.Length;
		uint result = 0u;
		if (!isMegabot)
		{
			for (uint num2 = 0u; num2 < num; num2++)
			{
				if (totalRobotRanking < tiersBands[num2])
				{
					continue;
				}
				if (num2 + 1 < num)
				{
					if (totalRobotRanking < tiersBands[num2 + 1])
					{
						result = num2;
					}
				}
				else
				{
					result = num2;
				}
			}
		}
		else
		{
			result = tiersData.MegabotTierIndex;
		}
		return result;
	}

	internal static uint GetTierLowerRRLimit(uint tierIndex, TiersData tiersData)
	{
		return tiersData.TiersBands[tierIndex];
	}

	internal static uint GetTierUpperRRLimit(uint tierIndex, TiersData tiersData)
	{
		if (!IsMegabotTier(tierIndex + 1, tiersData))
		{
			return tiersData.TiersBands[tierIndex + 1];
		}
		return tiersData.MaxRobotRankingARobotCanObtain;
	}

	internal static int GetTiersCount(TiersData tiersData)
	{
		return tiersData.TiersBands.Length + 1;
	}

	internal static string GetRankDisplayableName(int rankIndex)
	{
		switch (rankIndex)
		{
		case 0:
			return StringTableBase<StringTable>.Instance.GetString("strBronze");
		case 1:
			return StringTableBase<StringTable>.Instance.GetString("strSilver");
		case 2:
			return StringTableBase<StringTable>.Instance.GetString("strGold");
		case 3:
			return StringTableBase<StringTable>.Instance.GetString("strDiamond");
		case 4:
			return StringTableBase<StringTable>.Instance.GetString("strProtonium");
		case 5:
			return StringTableBase<StringTable>.Instance.GetString("strProtonium5");
		default:
			throw new Exception("Unknown rank");
		}
	}

	internal static string GetRankDisplayableNameForAnalytics(int rankIndex)
	{
		switch (rankIndex)
		{
		case 0:
			return "Bronze";
		case 1:
			return "Silver";
		case 2:
			return "Gold";
		case 3:
			return "Diamond";
		case 4:
			return "Protonium";
		case 5:
			return "Protonium5";
		default:
			throw new Exception("Unknown rank");
		}
	}

	internal static string ConvertTierIndexToTierString(uint tierIndex, bool isMegabot)
	{
		string str = "T ";
		if (!isMegabot)
		{
			return str + (tierIndex + 1).ToString();
		}
		return str + "M";
	}

	internal static string ConvertTierIndexToTierString(uint tierIndex, TiersData tiersData)
	{
		return ConvertTierIndexToTierString(tierIndex, IsMegabotTier(tierIndex, tiersData));
	}
}
