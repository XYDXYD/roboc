internal class PlayerRobotMasteryComponent : IPlayerRobotMasteryComponent
{
	public int masteryLevel
	{
		get;
		private set;
	}

	public PlayerRobotMasteryComponent(int masteryLevel_)
	{
		masteryLevel = masteryLevel_;
	}
}
