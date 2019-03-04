using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class HomingProjectileEngine : IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IPhysicallyTickable, IEngine, ITickableBase
	{
		private const float RESPONSIVITY = 10f;

		private WeaponRaycastUtility.Ray _raycastRay = default(WeaponRaycastUtility.Ray);

		private HitResult _hitResult = default(HitResult);

		private WeaponRaycastUtility.Parameters _raycastParameters;

		private NetworkPlayerBlinkedObserver _playerBlinkedObserver;

		private RemotePlayerBecomeInvisibleObserver _remotePlayerBecomeInvisibleObserver;

		[Inject]
		internal NetworkMachineManager networkMachineManager
		{
			get;
			private set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			private set;
		}

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			private set;
		}

		[Inject]
		internal LivePlayersContainer livePlayersContainer
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe HomingProjectileEngine(NetworkPlayerBlinkedObserver playerBlinkedObserver, RemotePlayerBecomeInvisibleObserver remotePlayerBecomeInvisibleObserver)
		{
			_playerBlinkedObserver = playerBlinkedObserver;
			_playerBlinkedObserver.AddAction(new ObserverAction<NetworkPlayerBlinkedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_remotePlayerBecomeInvisibleObserver = remotePlayerBecomeInvisibleObserver;
			_remotePlayerBecomeInvisibleObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			_raycastParameters = new WeaponRaycastUtility.Parameters
			{
				machineRootContainer = machineRootContainer,
				playerTeamsContainer = playerTeamsContainer,
				playerMachinesContainer = playerMachinesContainer,
				machineManager = networkMachineManager
			};
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<HomingProjectileNode> val = entityViewsDB.QueryEntityViews<HomingProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				HomingProjectileNode homingProjectileNode = val.get_Item(i);
				IProjectileOwnerComponent projectileOwnerComponent = homingProjectileNode.projectileOwnerComponent;
				_raycastParameters.Inject(projectileOwnerComponent.ownerId, projectileOwnerComponent.isAi, (!projectileOwnerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
				UpdateProjectile(homingProjectileNode, ref _raycastParameters);
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_playerBlinkedObserver.RemoveAction(new ObserverAction<NetworkPlayerBlinkedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_remotePlayerBecomeInvisibleObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnRemotePlayerBecomeInvisible(ref int playerId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<HomingProjectileNode> val = entityViewsDB.QueryEntityViews<HomingProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				HomingProjectileNode homingProjectileNode = val.get_Item(i);
				if (homingProjectileNode.lockOnComponent.targetPlayerId == playerId)
				{
					homingProjectileNode.lockOnComponent.hasAcquiredLock = false;
				}
			}
		}

		private void HandlePlayerBlinked(ref NetworkPlayerBlinkedData data)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<HomingProjectileNode> val = entityViewsDB.QueryEntityViews<HomingProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				HomingProjectileNode homingProjectileNode = val.get_Item(i);
				if (homingProjectileNode.lockOnComponent.targetPlayerId == data.playerId)
				{
					homingProjectileNode.lockOnComponent.hasAcquiredLock = false;
				}
			}
		}

		private void UpdateProjectile(HomingProjectileNode projectile, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			if (!projectile.projectileAliveComponent.active)
			{
				return;
			}
			IProjectileTimeComponent projectileTimeComponent = projectile.projectileTimeComponent;
			IProjectileAliveComponent projectileAliveComponent = projectile.projectileAliveComponent;
			float num = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
			Transform t = projectile.transformComponent.T;
			if (projectileAliveComponent.justFired)
			{
				IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
				IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
				projectileAliveComponent.justFired = false;
				Vector3 velocity = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, projectileOwnerComponent.machineId).get_velocity();
				IProjectileMovementStatsComponent projectileMovementStatsComponent = projectileMovementStats;
				projectileMovementStatsComponent.startPosition += num * velocity;
				t.set_position(projectileMovementStats.startPosition);
				IHomingProjectileStatsComponent rocketLauncherProjectileStats = projectile.rocketLauncherProjectileStats;
				rocketLauncherProjectileStats.angularVelocity = Vector3.get_zero();
				if (projectile.lockOnComponent.hasAcquiredLock && rocketLauncherProjectileStats.initialRotationSpeedRad > 0f && livePlayersContainer.IsPlayerAlive(TargetType.Player, projectile.lockOnComponent.targetPlayerId) && machineRootContainer.IsMachineRegistered(TargetType.Player, projectile.lockOnComponent.targetMachineId))
				{
					Vector3 lockWorldPosition = GetLockWorldPosition(projectile.lockOnComponent.targetMachineId, projectile.lockOnComponent.lockedCubePosition);
					Vector3 val = lockWorldPosition - t.get_position();
					Vector3 normalized = val.get_normalized();
					Vector3 val3 = rocketLauncherProjectileStats.angularVelocity = Vector3.Cross(t.get_forward(), normalized) * 10f * 3f;
					Vector3 angularVelocity = rocketLauncherProjectileStats.angularVelocity;
					if (angularVelocity.get_sqrMagnitude() > rocketLauncherProjectileStats.initialRotationSpeedRad * rocketLauncherProjectileStats.initialRotationSpeedRad)
					{
						Vector3 angularVelocity2 = rocketLauncherProjectileStats.angularVelocity;
						Vector3 normalized2 = angularVelocity2.get_normalized();
						rocketLauncherProjectileStats.angularVelocity = normalized2 * rocketLauncherProjectileStats.initialRotationSpeedRad;
					}
				}
			}
			Vector3 val4 = CurrentPosition(Time.get_deltaTime(), num, t, projectile);
			if (!CheckForCollison(projectile, num, t.get_position(), val4, ref raycastParams))
			{
				t.set_position(val4);
			}
		}

		private Vector3 CurrentPosition(float delta, float timeSinceLaunch, Transform t, HomingProjectileNode projectile)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			if (projectile.lockOnComponent.hasAcquiredLock)
			{
				if (livePlayersContainer.IsPlayerAlive(TargetType.Player, projectile.lockOnComponent.targetPlayerId) && machineRootContainer.IsMachineRegistered(TargetType.Player, projectile.lockOnComponent.targetMachineId))
				{
					IHomingProjectileStatsComponent rocketLauncherProjectileStats = projectile.rocketLauncherProjectileStats;
					Vector3 lockWorldPosition = GetLockWorldPosition(projectile.lockOnComponent.targetMachineId, projectile.lockOnComponent.lockedCubePosition);
					Vector3 val = lockWorldPosition - t.get_position();
					Vector3 normalized = val.get_normalized();
					Vector3 val2 = Vector3.Cross(t.get_forward(), normalized) * 10f;
					Vector3 val3 = (val2 - rocketLauncherProjectileStats.angularVelocity) / delta;
					float sqrMagnitude = val2.get_sqrMagnitude();
					Vector3 angularVelocity = rocketLauncherProjectileStats.angularVelocity;
					if (sqrMagnitude > angularVelocity.get_sqrMagnitude() && val3.get_sqrMagnitude() > rocketLauncherProjectileStats.maxRotationAccelerationRad * rocketLauncherProjectileStats.maxRotationAccelerationRad)
					{
						val3 = val3.get_normalized() * rocketLauncherProjectileStats.maxRotationAccelerationRad;
					}
					IHomingProjectileStatsComponent homingProjectileStatsComponent = rocketLauncherProjectileStats;
					homingProjectileStatsComponent.angularVelocity += val3 * delta;
					Vector3 angularVelocity2 = rocketLauncherProjectileStats.angularVelocity;
					Vector3 normalized2 = angularVelocity2.get_normalized();
					Vector3 angularVelocity3 = rocketLauncherProjectileStats.angularVelocity;
					if (angularVelocity3.get_sqrMagnitude() > rocketLauncherProjectileStats.maxRotationSpeedRad * rocketLauncherProjectileStats.maxRotationSpeedRad)
					{
						rocketLauncherProjectileStats.angularVelocity = normalized2 * rocketLauncherProjectileStats.maxRotationSpeedRad;
					}
					Vector3 val4 = normalized2;
					Vector3 angularVelocity4 = rocketLauncherProjectileStats.angularVelocity;
					t.Rotate(val4, angularVelocity4.get_magnitude() * delta * 57.29578f, 0);
				}
				else
				{
					projectile.lockOnComponent.hasAcquiredLock = false;
				}
				Vector3 val5 = t.get_forward() * projectile.projectileMovementStats.speed * delta;
				return t.get_position() + val5;
			}
			Vector3 val6 = projectile.projectileMovementStats.startDirection * projectile.projectileMovementStats.speed * delta;
			return t.get_position() + val6;
		}

		private bool CheckForCollison(HomingProjectileNode projectile, float timeElapsed, Vector3 previousPosition, Vector3 currentPosition, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			float num = timeElapsed * projectile.projectileMovementStats.speed;
			if (num > projectile.projectileRangeComponent.maxRange)
			{
				DisableProjectile(projectile);
				return true;
			}
			if (CheckForImpact(projectile, previousPosition, currentPosition, ref raycastParams))
			{
				DisableProjectile(projectile);
				return true;
			}
			return false;
		}

		private void DisableProjectile(HomingProjectileNode node)
		{
			node.projectileAliveComponent.active = false;
			int value = node.get_ID();
			node.projectileAliveComponent.resetProjectile.Dispatch(ref value);
		}

		private bool CheckForImpact(HomingProjectileNode projectile, Vector3 currentPosition, Vector3 nextPosition, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			CreateRay(currentPosition, nextPosition, out Vector3 direction, out float _, ref _raycastRay);
			_hitResult.hitPoint = currentPosition;
			_hitResult.normal = -direction;
			if (WeaponRaycastUtility.RaycastWeaponHit(ref _raycastRay, ref _raycastRay, ref raycastParams, ref _hitResult))
			{
				projectile.transformComponent.T.set_position(_hitResult.hitPoint);
				if (projectile.entitySourceComponent.isLocal)
				{
					projectile.weaponDamageComponent.hitResults = new HitResult[1]
					{
						_hitResult
					};
					projectile.weaponDamageComponent.numHits = 1;
					int value = projectile.get_ID();
					projectile.weaponDamageComponent.weaponDamage.Dispatch(ref value);
				}
				return true;
			}
			return false;
		}

		private void CreateRay(Vector3 currentPosition, Vector3 nextPosition, out Vector3 direction, out float distance, ref WeaponRaycastUtility.Ray raycastRay)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			direction = nextPosition - currentPosition;
			distance = direction.get_magnitude();
			direction /= distance;
			raycastRay.startPosition = currentPosition;
			raycastRay.direction = direction;
			raycastRay.range = distance;
		}

		private Vector3 GetLockWorldPosition(int targetMachineId, Byte3 cubePosition)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, targetMachineId).get_transform();
			return transform.get_position() + transform.get_rotation() * GridScaleUtility.GridToWorld(cubePosition, TargetType.Player);
		}
	}
}
