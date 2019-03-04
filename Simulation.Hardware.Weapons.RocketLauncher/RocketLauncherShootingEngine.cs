using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherShootingEngine : SingleEntityViewEngine<RocketLauncherShootingNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private WeaponFiredObservable _weaponFiredObservable;

		private Dictionary<int, LockOnData> _lockOnData = new Dictionary<int, LockOnData>(30);

		private Queue<GameObject> _muzzleReloadQueue = new Queue<GameObject>();

		private float _nextReloadTime;

		private float _reloadAnimationDelay = 0.2f;

		[Inject]
		internal ProjectileFactory projectileFactory
		{
			get;
			private set;
		}

		[Inject]
		internal MachineClusterContainer machineCluster
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe RocketLauncherShootingEngine(WeaponFiredObservable weaponFiredObservable, LockOnStateObserver lockStateObserver)
		{
			_weaponFiredObservable = weaponFiredObservable;
			lockStateObserver.AddAction(new ObserverAction<LockOnData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Tick(float deltaSec)
		{
			if (_muzzleReloadQueue.Count != 0 && Time.get_time() > _nextReloadTime)
			{
				GameObject val = _muzzleReloadQueue.Dequeue();
				val.SetActive(true);
				if (_muzzleReloadQueue.Count != 0)
				{
					_nextReloadTime = Time.get_time() + _reloadAnimationDelay;
				}
			}
		}

		protected override void Add(RocketLauncherShootingNode node)
		{
			node.projectileCreationComponent.createProjectile.subscribers += CreateProjectile;
			_lockOnData[node.weaponOwner.ownerId] = new LockOnData(0, 0, 0, hasAcquiredLock: false, new Byte3(0, 0, 0));
		}

		protected override void Remove(RocketLauncherShootingNode node)
		{
			node.projectileCreationComponent.createProjectile.subscribers -= CreateProjectile;
			_lockOnData.Remove(node.weaponOwner.ownerId);
		}

		private void HandleOnLockStateChange(ref LockOnData lockOnData)
		{
			_lockOnData[lockOnData.shooterId] = lockOnData;
		}

		private void CreateProjectile(IProjectileCreationComponent sender, ProjectileCreationParams projectileCreationParams)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			int value = projectileCreationParams.weaponId;
			bool isLocal = projectileCreationParams.isLocal;
			RocketLauncherShootingNode rocketLauncherShootingNode = default(RocketLauncherShootingNode);
			if (!entityViewsDB.TryQueryEntityView<RocketLauncherShootingNode>(value, ref rocketLauncherShootingNode))
			{
				return;
			}
			Vector3 launchPosition = WeaponAimUtility.GetMuzzlePosition(rocketLauncherShootingNode.muzzleFlashComponent);
			Vector3 direction = sender.launchDirection;
			LockOnData projectileParamaters = _lockOnData[rocketLauncherShootingNode.weaponOwner.ownerId];
			RocketLauncherProjectileMonoBehaviour rocketLauncherProjectileMonoBehaviour = projectileFactory.Build(rocketLauncherShootingNode, ref launchPosition, ref direction, ref isLocal) as RocketLauncherProjectileMonoBehaviour;
			rocketLauncherProjectileMonoBehaviour.SetProjectileParamaters(projectileParamaters);
			rocketLauncherShootingNode.afterEffectsComponent.applyRecoil.Dispatch(ref value);
			rocketLauncherShootingNode.afterEffectsComponent.playMuzzleFlash.Dispatch(ref value);
			rocketLauncherShootingNode.afterEffectsComponent.playFiringSound.Dispatch(ref value);
			if (rocketLauncherShootingNode.weaponOwner.ownedByMe)
			{
				float weaponFireCost = rocketLauncherShootingNode.weaponFireCostComponent.weaponFireCost;
				_weaponFiredObservable.Dispatch(ref weaponFireCost);
				rocketLauncherShootingNode.robotShakeComponent.applyShake.set_value(value);
				rocketLauncherShootingNode.cameraShakeComponent.applyShake.set_value(value);
			}
			if (rocketLauncherShootingNode.muzzleFlashComponent.hasRocketReloadAnim)
			{
				GameObject rocketReloadObject = rocketLauncherShootingNode.muzzleFlashComponent.rocketReloadObject;
				rocketReloadObject.SetActive(false);
				_muzzleReloadQueue.Enqueue(rocketReloadObject);
				if (_nextReloadTime < Time.get_time())
				{
					_nextReloadTime = Time.get_time() + _reloadAnimationDelay;
				}
			}
		}

		public void Ready()
		{
		}
	}
}
