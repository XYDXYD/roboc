using Svelto.ECS;

internal class PlayerTargetNode : EntityView
{
	public IPlayerTargetGameObjectComponent playerTargetGameObjectComponent;

	public IPlayerRobotMasteryComponent playerRobotMasteryComponent;

	public PlayerTargetNode()
		: this()
	{
	}
}
