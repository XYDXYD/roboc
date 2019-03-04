using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponProjectileMonoBehaviour : MonoBehaviour, IWeaponProjectilePrefabComponent, IImplementor
	{
		public GameObject projectilePrefab;

		public GameObject projectilePrefabEnemy;

		GameObject IWeaponProjectilePrefabComponent.projectilePrefab
		{
			get
			{
				return projectilePrefab;
			}
		}

		GameObject IWeaponProjectilePrefabComponent.projectilePrefabEnemy
		{
			get
			{
				return projectilePrefabEnemy;
			}
		}

		public WeaponProjectileMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
		}
	}
}
