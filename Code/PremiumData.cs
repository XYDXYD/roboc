using System;

internal sealed class PremiumData
{
	public TimeSpan premiumTimeLeft = TimeSpan.MaxValue;

	public DateTime premiumLoadTime = DateTime.MaxValue;

	public bool hasPremiumForLife;
}
