using System.Collections.Generic;

internal class ClaimPendingPromotionsResponse
{
	public string[] Promotions
	{
		get;
		private set;
	}

	public Dictionary<string, object> SteamPromotions
	{
		get;
		private set;
	}

	public Dictionary<string, object> CubesAwarded
	{
		get;
		private set;
	}

	public long CosmeticCreditsAwarded
	{
		get;
		private set;
	}

	public bool RoboPassAwarded
	{
		get;
		private set;
	}

	public ClaimPendingPromotionsResponse(string[] promotions, Dictionary<string, object> steamPromotions, Dictionary<string, object> cubesAwarded, long cosmeticCreditsAwarded, bool roboPassAwarded)
	{
		Promotions = promotions;
		SteamPromotions = steamPromotions;
		CubesAwarded = cubesAwarded;
		CosmeticCreditsAwarded = cosmeticCreditsAwarded;
		RoboPassAwarded = roboPassAwarded;
	}
}
