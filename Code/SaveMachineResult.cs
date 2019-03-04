internal class SaveMachineResult
{
	public int errorCode;

	public uint garageSlotId;

	public SaveMachineResult(int e, uint id)
	{
		errorCode = e;
		garageSlotId = id;
	}
}
