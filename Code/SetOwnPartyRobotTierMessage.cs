internal class SetOwnPartyRobotTierMessage
{
	internal readonly int PlayerRobotTier;

	internal readonly string RobotTierDisplayString;

	internal SetOwnPartyRobotTierMessage(int playerTier_, string displayString_)
	{
		PlayerRobotTier = playerTier_;
		RobotTierDisplayString = displayString_;
	}
}
