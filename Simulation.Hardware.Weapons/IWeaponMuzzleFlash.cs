using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponMuzzleFlash
	{
		GameObject muzzleFlashPrefab
		{
			get;
		}

		GameObject muzzleFlashPrefabEnemy
		{
			get;
		}

		List<Transform> muzzleFlashLocations
		{
			get;
		}

		int activeMuzzleFlash
		{
			get;
			set;
		}

		GameObject rocketReloadObject
		{
			get;
		}

		bool hasRocketReloadAnim
		{
			get;
		}
	}
}
