using Simulation.Hardware.Weapons;
using System.Collections.Generic;

namespace Simulation
{
	internal interface IWeaponFireStateSyncDependency
	{
		float timeStamp
		{
			get;
		}

		List<HitCubeInfo> hitCubeInfo
		{
			get;
		}

		int hitMachineId
		{
			get;
		}

		TargetType targetType
		{
			get;
		}

		void MinorIncreaseToTimeStamp();
	}
}
