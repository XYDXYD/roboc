internal class CustomGameTierChangedMessage
{
	public string TargetUser;

	public int Tier;

	public string TierString;

	internal CustomGameTierChangedMessage(int tier_, string targetUser_, string tierString_)
	{
		TargetUser = targetUser_;
		Tier = tier_;
		TierString = tierString_;
	}

	internal CustomGameTierChangedMessage(string targetUser_)
	{
		TargetUser = targetUser_;
		Tier = 0;
		TierString = string.Empty;
	}
}
