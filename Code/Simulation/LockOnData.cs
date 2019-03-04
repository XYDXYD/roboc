namespace Simulation
{
	internal struct LockOnData
	{
		public int shooterId;

		public int targetPlayerId;

		public int targetMachineId;

		public bool hasAcquiredLock;

		public Byte3 lockedCubePosition;

		public LockOnData(int shooterId, int targetPlayerId, int targetMachineId, bool hasAcquiredLock, Byte3 lockedCubePosition)
		{
			this.shooterId = shooterId;
			this.targetPlayerId = targetPlayerId;
			this.targetMachineId = targetMachineId;
			this.hasAcquiredLock = hasAcquiredLock;
			this.lockedCubePosition = lockedCubePosition;
		}
	}
}
