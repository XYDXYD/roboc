using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponProjectilePrefabComponent
	{
		GameObject projectilePrefab
		{
			get;
		}

		GameObject projectilePrefabEnemy
		{
			get;
		}
	}
}
