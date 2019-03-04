public struct NetworkStunnedMachineData
{
	public int machineId;

	public bool isStunned;

	public NetworkStunnedMachineData(int machineId_, bool isStunned_)
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
