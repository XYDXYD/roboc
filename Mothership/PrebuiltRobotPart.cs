namespace Mothership
{
	internal sealed class PrebuiltRobotPart
	{
		public string id;

		public MachineModel machineModel;

		public RobotPartData robotPartData;

		public PrebuiltRobotPart(string id_, RobotPartData robotPartData_)
		{
			id = id_;
			robotPartData = robotPartData_;
		}
	}
}
