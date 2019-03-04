namespace Simulation
{
	public struct StunnedMachineEffectData
	{
		public int locatorId;

		public int machineId;

		public StunnedMachineEffectData(int locatorId_, int machineId_)
		{
			locatorId = locatorId_;
			machineId = machineId_;
		}

		public void SetValues(int locatorId_, int machineId_)
		{
			locatorId = locatorId_;
			machineId = machineId_;
		}
	}
}
