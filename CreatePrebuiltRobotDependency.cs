internal class CreatePrebuiltRobotDependency
{
	public byte[] robotData
	{
		get;
		private set;
	}

	public byte[] robotColourData
	{
		get;
		private set;
	}

	public CreatePrebuiltRobotDependency(byte[] robotData_, byte[] robotColourData_)
	{
		robotData = robotData_;
		robotColourData = robotColourData_;
	}
}
