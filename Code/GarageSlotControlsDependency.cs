internal class GarageSlotControlsDependency
{
	public readonly uint index;

	public readonly ControlSettings controlSetting;

	public readonly bool isReadOnlyRobot;

	public GarageSlotControlsDependency(uint i, ControlSettings setting)
	{
		index = i;
		controlSetting = setting;
	}
}
