internal class UpdateShopRobotOffsetDependency
{
	public int cubesOffsetX;

	public int cubesOffsetZ;

	public int expectedLocationFirstCubeX;

	public int expectedLocationFirstCubeY;

	public int expectedLocationFirstCubeZ;

	public long robotId
	{
		get;
		set;
	}

	public UpdateShopRobotOffsetDependency()
	{
		robotId = -1L;
		cubesOffsetX = 0;
		cubesOffsetZ = 0;
		expectedLocationFirstCubeX = 0;
		expectedLocationFirstCubeY = 0;
		expectedLocationFirstCubeZ = 0;
	}

	public UpdateShopRobotOffsetDependency(UpdateShopRobotOffsetDependency source)
	{
		robotId = source.robotId;
		cubesOffsetX = source.cubesOffsetX;
		cubesOffsetZ = source.cubesOffsetZ;
		expectedLocationFirstCubeX = source.expectedLocationFirstCubeX;
		expectedLocationFirstCubeY = source.expectedLocationFirstCubeY;
		expectedLocationFirstCubeZ = source.expectedLocationFirstCubeZ;
	}
}
