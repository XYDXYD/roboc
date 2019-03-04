namespace Mothership
{
	public class CreateNewRobotDependency
	{
		public readonly CreateNewRobotType createNewRobotType;

		public readonly byte[] robotCubeData;

		public CreateNewRobotDependency(CreateNewRobotType createNewRobotType_)
		{
			createNewRobotType = createNewRobotType_;
		}

		public CreateNewRobotDependency(CreateNewRobotType createNewRobotType_, byte[] robotCubeData_)
		{
			createNewRobotType = createNewRobotType_;
			robotCubeData = robotCubeData_;
		}
	}
}
