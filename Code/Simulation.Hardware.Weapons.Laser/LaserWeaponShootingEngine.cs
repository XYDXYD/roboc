using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Laser
{
	internal sealed class LaserWeaponShootingEngine : SingleEntityViewEngine<LaserWeaponShootingNode>
	{
		private WeaponFiredObservable _weaponFiredObservable;

		private Dictionary<int, LaserWeaponShootingNode> _weapons = new Dictionary<int, LaserWeaponShootingNode>();

		[Inject]
		internal ProjectileFactory projectileFactory
		{
			get;
			private set;
		}

		public LaserWeaponShootingEngine(WeaponFiredObservable weaponFiredObservable)
		{
			_weaponFiredObservable = weaponFiredObservable;
		}

		protected override void Add(LaserWeaponShootingNode obj)
		{
			obj.projectileCreationComponent.createProjectile.subscribers += CreateProjectile;
			_weapons.Add(obj.get_ID(), obj);
		}

		protected override void Remove(LaserWeaponShootingNode obj)
		{
			obj.projectileCreationComponent.createProjectile.subscribers -= CreateProjectile;
			_weapons.Remove(obj.get_ID());
		}

		private void CreateProjectile(IProjectileCreationComponent sender, ProjectileCreationParams projectileCreationParams)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			int value = projectileCreationParams.weaponId;
			bool isLocal = projectileCreationParams.isLocal;
			if (!_weapons.TryGetValue(value, out LaserWeaponShootingNode value2))
			{
				return;
			}
			Vector3 launchPosition = WeaponAimUtility.GetMuzzlePosition(value2.muzzleFlashComponent);
			Vector3 direction = sender.launchDirection;
			projectileFactory.Build(value2, ref launchPosition, ref direction, ref isLocal);
			value2.afterEffectsComponent.applyRecoil.Dispatch(ref value);
			value2.afterEffectsComponent.playMuzzleFlash.Dispatch(ref value);
			value2.afterEffectsComponent.playFiringSound.Dispatch(ref value);
			if (value2.weaponOwner.ownedByMe)
			{
				float num = value2.weaponFireCostComponent.weaponFireCost;
				if (value2.itemCategory.itemCategory == ItemCategory.Chaingun)
				{
					num = 0f;
				}
				_weaponFiredObservable.Dispatch(ref num);
				value2.robotShakeComponent.applyShake.set_value(value);
				value2.cameraShakeComponent.applyShake.set_value(value);
			}
		}
	}
}
