using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IMachineInfo
	{
		GameObject machineRoot
		{
			get;
			set;
		}

		int playerId
		{
			get;
			set;
		}

		int machineId
		{
			get;
			set;
		}

		WeaponRaycast weaponRaycast
		{
			get;
			set;
		}
	}
}
