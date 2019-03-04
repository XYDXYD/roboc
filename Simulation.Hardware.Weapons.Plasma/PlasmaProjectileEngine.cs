using Battle;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal sealed class PlasmaProjectileEngine : SingleEntityViewEngine<PlasmaProjectileNode>, IInitialize, IPhysicallyTickable, ITickableBase
	{
		internal struct CubesHitData
		{
			public InstantiatedCube cubeHit;

			public int damage;

			public int machineId;
		}

		private BroadcastMissClientCommand _broadcastMissClientCommand;

		private WeaponFireNoEffectClientCommand _weaponFireNoEffectClientCommand;

		private WeaponFireEffectOnlyClientCommand _weaponFireEffectOnlyClientCommand;

		private FireMissDependency _fireMissDependency = new FireMissDependency();

		private DestroyCubeNoEffectDependency _weaponFireNoEffectDependency = new DestroyCubeNoEffectDependency();

		private DestroyCubeEffectOnlyDependency _weaponFireEffectOnlyDependency = new DestroyCubeEffectOnlyDependency();

		private Dictionary<int, PlasmaProjectileNode> _projectiles = new Dictionary<int, PlasmaProjectileNode>();

		private Dictionary<int, WeaponRaycastUtility.Parameters> _raycastParameters = new Dictionary<int, WeaponRaycastUtility.Parameters>();

		private List<HitCubeInfo> _destroyedCubes = new List<HitCubeInfo>(30);

		private Dictionary<InstantiatedCube, int> _proposedDestroyedCubes = new Dictionary<InstantiatedCube, int>(30);

		[Inject]
		internal NetworkMachineManager networkMachineManager
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
		internal CubeDamagePropagator cubeDamagePropagator
		{
			get;
			private set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
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
		internal BattleTimer battleTimer
		{
			get;
			private set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireNoEffectClientCommand = commandFactory.Build<WeaponFireNoEffectClientCommand>();
			_weaponFireEffectOnlyClientCommand = commandFactory.Build<WeaponFireEffectOnlyClientCommand>();
			_broadcastMissClientCommand = commandFactory.Build<BroadcastMissClientCommand>();
		}

		protected override void Add(PlasmaProjectileNode obj)
		{
			if (obj != null)
			{
				_projectiles.Add(obj.get_ID(), obj);
				obj.transformComponent.T.get_gameObject().SetActive(false);
				IProjectileOwnerComponent ownerComponent = obj.ownerComponent;
				WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
				parameters.machineRootContainer = machineRootContainer;
				parameters.playerTeamsContainer = playerTeamsContainer;
				parameters.playerMachinesContainer = playerMachinesContainer;
				parameters.machineManager = networkMachineManager;
				parameters.fusionShieldTag = ((!ownerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
				parameters.shooterId = ownerComponent.ownerId;
				parameters.isShooterAi = ownerComponent.isAi;
				WeaponRaycastUtility.Parameters value = parameters;
				_raycastParameters.Add(obj.get_ID(), value);
			}
		}

		protected override void Remove(PlasmaProjectileNode obj)
		{
			if (obj != null)
			{
				int iD = obj.get_ID();
				_projectiles.Remove(iD);
				_raycastParameters.Remove(iD);
			}
		}

		public void PhysicsTick(float deltaSec)
		{
			Dictionary<int, PlasmaProjectileNode>.Enumerator enumerator = _projectiles.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlasmaProjectileNode value = enumerator.Current.Value;
				WeaponRaycastUtility.Parameters raycastParams = _raycastParameters[enumerator.Current.Key];
				raycastParams.Inject(value.ownerComponent.ownerId, value.ownerComponent.isAi, (!value.ownerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
				UpdateProjectile(enumerator.Current.Value, ref raycastParams);
			}
		}

		private void PerformExplosionEffect(PlasmaProjectileNode projectile, HitResult hitResult, bool isEnemy, bool damagedCube, bool targetIsMe)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			IProjectileOwnerComponent ownerComponent = projectile.ownerComponent;
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			bool ownedByMe = ownerComponent.ownedByMe;
			Vector3 hitPoint = hitResult.hitPoint;
			Vector3 normal = hitResult.normal;
			bool hitSelf = hitResult.hitSelf;
			TargetType targetType = hitResult.targetType;
			HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.ownerComponent.isEnemy, hit_: true, hitResult.hitSelf, hitPoint, Quaternion.get_identity(), normal, targetIsMe, ownedByMe);
			if (hitSelf && !damagedCube)
			{
				hitSomethingComponent.hitSelf.Dispatch(ref value);
				return;
			}
			if (damagedCube)
			{
				hitSomethingComponent.hitEnemy.Dispatch(ref value);
				if (!hitSelf)
				{
					hitSomethingComponent.hitSecondaryImpact.Dispatch(ref value);
				}
			}
			if (targetType == TargetType.TeamBase && !hitResult.hitOwnBase)
			{
				hitSomethingComponent.hitProtonium.Dispatch(ref value);
				return;
			}
			switch (targetType)
			{
			case TargetType.FusionShield:
				hitSomethingComponent.hitFusionShield.Dispatch(ref value);
				return;
			case TargetType.EqualizerCrystal:
				if (!hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitEqualizer.Dispatch(ref value);
					return;
				}
				break;
			}
			if (!damagedCube)
			{
				hitSomethingComponent.hitEnvironment.Dispatch(ref value);
			}
		}

		private void UpdateProjectile(PlasmaProjectileNode plasmaProjectile, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			Transform t = plasmaProjectile.transformComponent.T;
			if (t.get_gameObject().get_activeInHierarchy() && plasmaProjectile.projectileAliveComponent.active)
			{
				IProjectileMovementStatsComponent projectileMovementStats = plasmaProjectile.projectileMovementStats;
				IProjectileTimeComponent projectileTimeComponent = plasmaProjectile.projectileTimeComponent;
				IProjectileOwnerComponent ownerComponent = plasmaProjectile.ownerComponent;
				IProjectileAliveComponent projectileAliveComponent = plasmaProjectile.projectileAliveComponent;
				IProjectileRangeComponent projectileRangeComponent = plasmaProjectile.projectileRangeComponent;
				IProjectileTimeComponent projectileTimeComponent2 = plasmaProjectile.projectileTimeComponent;
				float num = Vector3.Distance(projectileMovementStats.startPosition, t.get_position());
				if (projectileAliveComponent.justFired)
				{
					float num2 = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
					projectileTimeComponent.startTime = Time.get_timeSinceLevelLoad();
					projectileAliveComponent.justFired = false;
					Vector3 position = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, ownerComponent.machineId).get_position();
					position += rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, ownerComponent.machineId).get_velocity() * num2;
					Vector3 robotStartPosition = projectileMovementStats.robotStartPosition;
					Vector3 val = position - robotStartPosition;
					IProjectileMovementStatsComponent projectileMovementStatsComponent = projectileMovementStats;
					projectileMovementStatsComponent.startPosition += val;
				}
				float num3 = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
				if (num > projectileRangeComponent.maxRange || num3 > projectileTimeComponent2.maxTime)
				{
					DisableProjectile(plasmaProjectile);
				}
				else if (!CheckForCollision(num3, plasmaProjectile, ref raycastParams))
				{
					t.set_position(CalculatePosition(num3, projectileMovementStats.startPosition, projectileMovementStats.startVelocity, plasmaProjectile.gravityComponent.gravity));
				}
			}
		}

		private Vector3 CalculatePosition(float timeElapsed, Vector3 startPosition, Vector3 startVelocity, Vector3 gravity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = startVelocity * timeElapsed;
			Vector3 val2 = 0.5f * gravity * timeElapsed * timeElapsed;
			return startPosition + val + val2;
		}

		private bool CheckForCollision(float timeElapsed, PlasmaProjectileNode projectile, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
			IProjectileTimeComponent projectileTimeComponent = projectile.projectileTimeComponent;
			Vector3 val = CalculatePosition(timeElapsed, projectileMovementStats.startPosition, projectileMovementStats.startVelocity, projectile.gravityComponent.gravity);
			Vector3 nextPos = CalculatePosition(timeElapsed + Time.get_fixedDeltaTime(), projectileMovementStats.startPosition, projectileMovementStats.startVelocity, projectile.gravityComponent.gravity);
			return CheckForImpact(val, val, nextPos, projectile, ref raycastParams, timeElapsed);
		}

		private bool CheckForImpact(Vector3 currentPos, Vector3 enemyPos, Vector3 nextPos, PlasmaProjectileNode projectile, ref WeaponRaycastUtility.Parameters raycastParams, float timeElapsed)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			WeaponRaycastUtility.Parameters parameters = raycastParams;
			Vector3 direction;
			float distance;
			WeaponRaycastUtility.Ray rayMe = CreateRay(currentPos, nextPos, out direction, out distance);
			HitResult hitResult = default(HitResult);
			hitResult.hitPoint = currentPos;
			hitResult.normal = -direction;
			HitResult hitResult2 = hitResult;
			WeaponRaycastUtility.Ray ray = CreateRay(enemyPos, nextPos, out direction, out distance);
			if (WeaponRaycastUtility.RaycastWeaponHit(ref rayMe, ref ray, ref parameters, ref hitResult2))
			{
				if (projectile.entitySourceComponent.isLocal)
				{
					DoProjectileHit(projectile, hitResult2, timeElapsed);
				}
				projectile.transformComponent.T.set_position(hitResult2.hitPoint);
				DisableProjectile(projectile);
				return true;
			}
			return false;
		}

		private void DoProjectileHit(PlasmaProjectileNode projectile, HitResult hitResult, float projectieTimeAlive)
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			IPlasmaProjectileStatsComponent plasmaProjectileComponent = projectile.plasmaProjectileComponent;
			IProjectileDamageStatsComponent projectileDamageStats = projectile.projectileDamageStats;
			IProjectileOwnerComponent ownerComponent = projectile.ownerComponent;
			float currentExplosionRadius = plasmaProjectileComponent.currentExplosionRadius;
			TargetType targetType = hitResult.targetType;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (targetType != TargetType.FusionShield && !hitResult.hitSelf)
			{
				WeaponSplashDamageUtility.HitCubesResultList nearestCubeWithinSphere = GetNearestCubeWithinSphere(projectile, currentExplosionRadius, ref hitResult);
				flag = ApplyDamage(projectile, ref hitResult, nearestCubeWithinSphere.playerMachines, TargetType.Player);
				flag2 = ApplyDamage(projectile, ref hitResult, nearestCubeWithinSphere.teamBases, TargetType.TeamBase);
				flag3 = ApplyDamage(projectile, ref hitResult, nearestCubeWithinSphere.equalizers, TargetType.EqualizerCrystal);
			}
			if (!flag && !flag2 && !flag3)
			{
				_fireMissDependency.SetVariables(ownerComponent.machineId, projectile.itemDescriptorComponent.itemDescriptor, hitResult.hitPoint, hitResult.normal, _hit: true, hitResult.hitSelf, battleTimer.SecondsSinceGameInitialised, targetType);
				_broadcastMissClientCommand.Inject(_fireMissDependency).Execute();
			}
			PerformExplosionEffect(projectile, hitResult, ownerComponent.isEnemy, flag || flag2 || flag3, targetIsMe: false);
		}

		private bool ApplyDamage(PlasmaProjectileNode projectile, ref HitResult hitResult, Dictionary<int, FasterList<InstantiatedCube>> targets, TargetType type)
		{
			bool result = false;
			bool flag = false;
			foreach (KeyValuePair<int, FasterList<InstantiatedCube>> target in targets)
			{
				int key = target.Key;
				FasterList<InstantiatedCube> value = target.Value;
				_destroyedCubes.Clear();
				ProcessHitCubes(value, projectile.projectileDamageStats);
				if (_destroyedCubes.Count > 0)
				{
					CreateFireCommandNoEffect(key, type, projectile);
					if (!flag)
					{
						CreateCubeDestroyEffectOnlyCommand(key, type, ref hitResult, projectile);
						flag = true;
					}
					result = true;
				}
			}
			return result;
		}

		private void CreateFireCommandNoEffect(int hitMachineId, TargetType targetType, PlasmaProjectileNode projectile)
		{
			if (_destroyedCubes.Count > 0)
			{
				_weaponFireNoEffectDependency.SetVariables(projectile.ownerComponent.machineId, hitMachineId, _destroyedCubes, battleTimer.SecondsSinceGameInitialised, targetType);
				_weaponFireNoEffectClientCommand.Inject(_weaponFireNoEffectDependency).Execute();
			}
		}

		private void CreateCubeDestroyEffectOnlyCommand(int hitMachineId, TargetType targetType, ref HitResult hitResult, PlasmaProjectileNode projectile)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (_destroyedCubes.Count > 0)
			{
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
				HitCubeInfo hitCubeInfo = _destroyedCubes[0];
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitResult.hitPoint - cubeWorldPosition;
				DestroyCubeEffectOnlyDependency weaponFireEffectOnlyDependency = _weaponFireEffectOnlyDependency;
				HitCubeInfo hitCubeInfo2 = _destroyedCubes[0];
				weaponFireEffectOnlyDependency.SetVariables(hitCubeInfo2.gridLoc, projectile.ownerComponent.machineId, hitMachineId, targetType, projectile.itemDescriptorComponent.itemDescriptor, hitEffectOffset, hitResult.normal);
				_weaponFireEffectOnlyClientCommand.Inject(_weaponFireEffectOnlyDependency);
				_weaponFireEffectOnlyClientCommand.Execute();
			}
		}

		private WeaponSplashDamageUtility.HitCubesResultList GetNearestCubeWithinSphere(PlasmaProjectileNode projectile, float explosionRadius, ref HitResult projectileHit)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			WeaponSplashDamageUtility.SplashParameters splashParameters = default(WeaponSplashDamageUtility.SplashParameters);
			splashParameters.position = projectileHit.hitPoint;
			splashParameters.radius = explosionRadius;
			splashParameters.direction = projectile.transformComponent.T.get_forward();
			splashParameters.additionalHits = projectile.splashComponent.additionalHits;
			splashParameters.coneAngle = projectile.splashComponent.coneAngle;
			WeaponSplashDamageUtility.Parameters parameters = default(WeaponSplashDamageUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.rigidbodyDataContainer = rigidbodyDataContainer;
			parameters.machineManager = networkMachineManager;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.projectileOwnerTeam = projectile.ownerComponent.ownerTeam;
			return WeaponSplashDamageUtility.SplashDamageCubesList(ref splashParameters, ref parameters, ref projectileHit);
		}

		private void ProcessHitCubes(FasterList<InstantiatedCube> hitCubes, IProjectileDamageStatsComponent damageStats)
		{
			_proposedDestroyedCubes.Clear();
			for (int num = hitCubes.get_Count() - 1; num >= 0; num--)
			{
				InstantiatedCube target = hitCubes.get_Item(num);
				int damage = Mathf.CeilToInt(damageStats.campaignDifficultyFactor * damageStats.damageBoost * damageStats.damageBuff * damageStats.damageMultiplier * (float)damageStats.damage / (float)hitCubes.get_Count());
				cubeDamagePropagator.ComputeProposedDamage(target, damage, damageStats.protoniumDamageScale, ref _proposedDestroyedCubes);
			}
			if (_proposedDestroyedCubes.Count > 0)
			{
				cubeDamagePropagator.GenerateDestructionGroupHitInfo(_proposedDestroyedCubes, _destroyedCubes);
			}
		}

		private WeaponRaycastUtility.Ray CreateRay(Vector3 currentPosition, Vector3 nextPosition, out Vector3 direction, out float distance)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			direction = nextPosition - currentPosition;
			distance = direction.get_magnitude();
			direction /= distance;
			WeaponRaycastUtility.Ray result = default(WeaponRaycastUtility.Ray);
			result.startPosition = currentPosition;
			result.direction = direction;
			result.range = distance;
			return result;
		}

		private void DisableProjectile(PlasmaProjectileNode node)
		{
			int value = node.get_ID();
			node.projectileAliveComponent.resetProjectile.Dispatch(ref value);
			node.projectileAliveComponent.active = false;
		}
	}
}
