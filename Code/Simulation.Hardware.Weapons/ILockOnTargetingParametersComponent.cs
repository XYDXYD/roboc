namespace Simulation.Hardware.Weapons
{
	internal interface ILockOnTargetingParametersComponent
	{
		float lockTime
		{
			get;
			set;
		}

		float fullLockReleaseTime
		{
			get;
			set;
		}

		float changeLockTime
		{
			get;
			set;
		}

		float lockOnConeDot
		{
			get;
		}

		bool isLooseLock
		{
			get;
		}

		bool notifyTargetOfLock
		{
			get;
		}
	}
}
