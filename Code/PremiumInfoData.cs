using System;

internal class PremiumInfoData
{
	public TimeSpan premiumTimeSpan
	{
		get;
		set;
	}

	public bool hasPremiumForLife
	{
		get;
		private set;
	}

	public bool HasPremium => hasPremiumForLife || premiumTimeSpan != TimeSpan.Zero;

	public PremiumInfoData(TimeSpan premiumTimeSpan_, bool hasPremiumForLife_)
	{
		premiumTimeSpan = premiumTimeSpan_;
		hasPremiumForLife = hasPremiumForLife_;
	}
}
