internal struct IncomeScalesResponse
{
	public readonly int PremiumXpBonusPercent;

	public readonly int PartyBonusPercentagePerPlayer;

	public readonly double BonusPerTierMultiplier;

	public IncomeScalesResponse(int premiumXpBonusPercent, int partyBonusPercentagePerPlayer, double bonusPerTierMultiplier)
	{
		PremiumXpBonusPercent = premiumXpBonusPercent;
		PartyBonusPercentagePerPlayer = partyBonusPercentagePerPlayer;
		BonusPerTierMultiplier = bonusPerTierMultiplier;
	}
}
