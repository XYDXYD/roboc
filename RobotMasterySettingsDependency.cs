using ExitGames.Client.Photon;

internal sealed class RobotMasterySettingsDependency
{
	public int robitsRewardForCRFRobotCreator;

	public RobotMasterySettingsDependency(Hashtable data)
	{
		robitsRewardForCRFRobotCreator = (int)data.get_Item((object)"RobitsRewardForCRFRobotCreator");
	}
}
