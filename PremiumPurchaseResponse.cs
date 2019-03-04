using System;

public class PremiumPurchaseResponse
{
	public int numPremiumDaysAwarded
	{
		get;
		private set;
	}

	public TimeSpan timeLeft
	{
		get;
		private set;
	}

	public bool hasPremiumForLife
	{
		get;
		private set;
	}

	public PremiumPurchaseResponse(TimeSpan timeLeft, bool hasPremiumForLife, int numPremiumDaysAwarded_)
	{
		this.timeLeft = timeLeft;
		this.hasPremiumForLife = hasPremiumForLife;
		numPremiumDaysAwarded = numPremiumDaysAwarded_;
	}
}
