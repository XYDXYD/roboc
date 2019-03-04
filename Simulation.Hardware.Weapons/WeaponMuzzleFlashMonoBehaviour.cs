using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponMuzzleFlashMonoBehaviour : MonoBehaviour, IWeaponMuzzleFlash, IImplementor
	{
		public GameObject muzzleFlashPrefab;

		public GameObject muzzleFlashPrefabEnemy;

		public List<Transform> muzzleFlashLocations;

		GameObject IWeaponMuzzleFlash.muzzleFlashPrefab
		{
			get
			{
				return muzzleFlashPrefab;
			}
		}

		GameObject IWeaponMuzzleFlash.muzzleFlashPrefabEnemy
		{
			get
			{
				return muzzleFlashPrefabEnemy;
			}
		}

		List<Transform> IWeaponMuzzleFlash.muzzleFlashLocations
		{
			get
			{
				return muzzleFlashLocations;
			}
		}

		int IWeaponMuzzleFlash.activeMuzzleFlash
		{
			get;
			set;
		}

		public GameObject rocketReloadObject
		{
			get
			{
				Transform val = muzzleFlashLocations[((IWeaponMuzzleFlash)this).activeMuzzleFlash];
				return val.GetChild(0).get_gameObject();
			}
		}

		public bool hasRocketReloadAnim
		{
			get
			{
				Transform val = muzzleFlashLocations[((IWeaponMuzzleFlash)this).activeMuzzleFlash];
				return val.get_transform().get_childCount() > 0;
			}
		}

		public WeaponMuzzleFlashMonoBehaviour()
			: this()
		{
		}
	}
}
