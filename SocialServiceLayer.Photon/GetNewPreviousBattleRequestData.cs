namespace SocialServiceLayer.Photon
{
	public class GetNewPreviousBattleRequestData
	{
		public readonly int newSeasonXP;

		public readonly int xpAwardBase;

		public readonly int xpAwardPremiumPart;

		public readonly int xpAwardPartyPart;

		public readonly int xpAwardTierPart;

		public readonly int robitsTotal;

		public readonly int averageSeasonXPForEveryone;

		public readonly int ClanTotalXP;

		public readonly float xpMultiplierForBrawl;

		public readonly int robits;

		public readonly int premiumRobits;

		public readonly float longPlayMultiplier;

		public GetNewPreviousBattleRequestData(int newSeasonXP_, int xpAwardBase_, int xpAwardPremiumPart_, int xpAwardPartyPart_, int xpAwardTierPart_, int robitsTotal_, int averageXpForEveryone_, int clanTotalXP_, float xpMultiplier_, int robits_, int premiumRobits_, float longPlayMultiplier_)
		{
			newSeasonXP = newSeasonXP_;
			xpAwardBase = xpAwardBase_;
			xpAwardPartyPart = xpAwardPartyPart_;
			xpAwardPremiumPart = xpAwardPremiumPart_;
			xpAwardTierPart = xpAwardTierPart_;
			robitsTotal = robitsTotal_;
			averageSeasonXPForEveryone = averageXpForEveryone_;
			ClanTotalXP = clanTotalXP_;
			xpMultiplierForBrawl = xpMultiplier_;
			robits = robits_;
			premiumRobits = premiumRobits_;
			longPlayMultiplier = longPlayMultiplier_;
		}
	}
}
