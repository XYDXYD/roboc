public struct MachineStunnedData
{
	public int machineId;

	public bool isStunned;

	public MachineStunnedData(int machineId_, bool isStunned_)
	{
		machineId = machineId_;
		isStunned = isStunned_;
	}

	public void SetValues(int machineId_, bool isStunned_)
	{
		machineId = machineId_;
		isStunned = isStunned_;
	}
}
