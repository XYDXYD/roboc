using Fabric;
using Svelto.Command;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class BasicShootingEngine : SingleEntityViewEngine<WeaponShootingNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private WeaponNoFireObserver _noFireObserver;

		private NetworkWeaponFiredObserver _networkWeaponFiredObservable;

		private WeaponFireEffectDependency _weaponFireEffectDependency = new WeaponFireEffectDependency();

		private WeaponFireEffectClientCommand _weaponFireEffectClientCommand;

		private Dictionary<int, WeaponShootingNode> _weapons = new Dictionary<int, WeaponShootingNode>();

		private GameObject _currentProjectilePrefab;

		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		internal ProjectileFactory projectileFactory
		{
			get;
			private set;
		}

		public unsafe BasicShootingEngine(WeaponNoFireObserver noFireObserver, NetworkWeaponFiredObserver networkWeaponFiredObservable)
		{
			_noFireObserver = noFireObserver;
			_networkWeaponFiredObservable = networkWeaponFiredObservable;
			_noFireObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_networkWeaponFiredObservable.AddAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireEffectClientCommand = commandFactory.Build<WeaponFireEffectClientCommand>();
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_noFireObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_networkWeaponFiredObservable.RemoveAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(WeaponShootingNode obj)
		{
			obj.shootingComponent.shotIsGoingToBeFired.subscribers += FireWeapon;
			_weapons.Add(obj.get_ID(), obj);
			PreallocateProjectiles(obj.itemCategory.itemCategory, obj.weaponOwner.isEnemy, obj.projectilePrefabComponent);
		}

		protected override void Remove(WeaponShootingNode obj)
		{
			obj.shootingComponent.shotIsGoingToBeFired.subscribers -= FireWeapon;
			_weapons.Remove(obj.get_ID());
		}

		private void PreallocateProjectiles(ItemCategory itemCategory, bool isEnemy, IWeaponProjectilePrefabComponent projectilePrefabComponent)
		{
			_currentProjectilePrefab = ((!isEnemy) ? projectilePrefabComponent.projectilePrefab : projectilePrefabComponent.projectilePrefabEnemy);
			projectileFactory.PreallocateProjectiles(_currentProjectilePrefab, itemCategory);
		}

		private void HandleRemoteFireEvent(ref FiringInfo firingInfo)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (_weapons.TryGetValue(firingInfo.weaponId, out WeaponShootingNode value))
			{
				value.aimingComponent.direction = firingInfo.direction;
				value.aimingComponent.targetPoint = firingInfo.targetPoint;
				value.projectileCreationComponent.launchDirection = firingInfo.direction;
				FireWeapon(value, isLocal: false);
			}
		}

		private void FireWeapon(IShootingComponent sender, int senderWeaponId)
		{
			FireWeapon(_weapons[senderWeaponId], isLocal: true);
		}

		private void HandleOnNoFirePressed(ref int weaponId)
		{
			if (_weapons.ContainsKey(weaponId))
			{
				PlayNoFiringSound(_weapons[weaponId]);
			}
		}

		private void FireWeapon(WeaponShootingNode weapon, bool isLocal)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			IHardwareOwnerComponent weaponOwner = weapon.weaponOwner;
			_currentProjectilePrefab = ((!weaponOwner.isEnemy) ? weapon.projectilePrefabComponent.projectilePrefab : weapon.projectilePrefabComponent.projectilePrefabEnemy);
			ProjectileCreationParams value = new ProjectileCreationParams(weapon.get_ID(), isLocal);
			weapon.projectileCreationComponent.createProjectile.Dispatch(ref value);
			weapon.muzzleFlashComponent.activeMuzzleFlash = (weapon.muzzleFlashComponent.activeMuzzleFlash + 1) % weapon.muzzleFlashComponent.muzzleFlashLocations.Count;
			if (weaponOwner.ownedByMe || weaponOwner.ownedByAi)
			{
				Vector3 muzzlePosition = WeaponAimUtility.GetMuzzlePosition(weapon.muzzleFlashComponent);
				SendFireEffectCommandToServer(muzzlePosition, weapon.projectileCreationComponent.launchDirection, weaponOwner.machineId, weaponOwner.ownerId, weapon.cubePositionComponent.gridPos);
			}
		}

		private void SendFireEffectCommandToServer(Vector3 launchPosition, Vector3 direction, int shootingMachine, int shootingPlayer, Byte3 gridPos)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_weaponFireEffectDependency.SetVariables(launchPosition, direction, shootingMachine, shootingPlayer, gridPos);
			_weaponFireEffectClientCommand.Inject(_weaponFireEffectDependency);
			_weaponFireEffectClientCommand.Execute();
		}

		private void PlayNoFiringSound(WeaponShootingNode shootingNode)
		{
			Transform t = shootingNode.transformComponent.T;
			string noFireAudio = shootingNode.noFireAudioComponent.noFireAudio;
			EventManager.get_Instance().PostEvent(noFireAudio, 0, (object)null, t.get_gameObject());
		}
	}
}
