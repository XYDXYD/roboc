internal class SaveMachineColorDependency
{
	public uint garageSlot;

	public byte[] colorMap;

	public SaveMachineColorDependency(uint garageSlot, byte[] colorMap)
	{
		this.garageSlot = garageSlot;
		this.colorMap = colorMap;
	}
}
