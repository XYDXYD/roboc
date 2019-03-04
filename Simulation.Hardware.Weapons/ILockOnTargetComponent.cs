namespace Simulation.Hardware.Weapons
{
	internal interface ILockOnTargetComponent
	{
		int targetPlayerId
		{
			get;
		}

		int targetMachineId
		{
			get;
		}

		bool hasAcquiredLock
		{
			get;
			set;
		}

		Byte3 lockedCubePosition
		{
			get;
		}
	}
}
