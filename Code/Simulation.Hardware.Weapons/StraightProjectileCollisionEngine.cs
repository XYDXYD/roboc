using Battle;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class StraightProjectileCollisionEngine : IInitialize, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private BroadcastMissClientCommand _broadcastMissClientCommand;

		private FireMissDependency _fireMissDependency = new FireMissDependency();

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
		internal CubeDamagePropagator cubeDamagePropagator
		{
			get;
			private set;
		}

		[Inject]
		internal CubeHealingPropagator cubeHealingPropagator
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
		internal BattleTimer battleTimer
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ProjectileNode> val = entityViewsDB.QueryEntityViews<ProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				UpdateProjectile(val.get_Item(i));
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			_broadcastMissClientCommand = commandFactory.Build<BroadcastMissClientCommand>();
		}

		private void UpdateProjectile(ProjectileNode laserProjectile)
		{
			if (laserProjectile.transformComponent.T.get_gameObject().get_activeInHierarchy() && laserProjectile.projectileAliveComponent.active && !laserProjectile.projectileStateComponent.disabled)
			{
				CheckForCollision(laserProjectile);
			}
		}

		private void CheckForCollision(ProjectileNode projectile)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			IPredictedProjectilePositionComponent predictedProjectilePosition = projectile.predictedProjectilePosition;
			CheckForImpact(predictedProjectilePosition.currentPos, predictedProjectilePosition.nextPos, projectile);
		}

		private void CheckForImpact(Vector3 currentPos, Vector3 nextPos, ProjectileNode projectile)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (CheckWeaponHit(projectile, out HitResult[] hitResults, out int numHits))
			{
				Vector3 hitPoint = hitResults[0].hitPoint;
				float num = Vector3.SqrMagnitude(projectile.projectileMovementStats.startPosition - hitPoint);
				float maxRange = projectile.projectileRangeComponent.maxRange;
				if (num < maxRange * maxRange)
				{
					projectile.weaponDamageComponent.hitResults = hitResults;
					projectile.weaponDamageComponent.numHits = numHits;
					CheckWeaponDamage(projectile);
					HandleProjectileHit(projectile, hitResults[0]);
				}
				projectile.transformComponent.T.set_position(hitPoint);
				projectile.projectileStateComponent.disabled = true;
			}
		}

		private bool CheckWeaponHit(ProjectileNode projectile, out HitResult[] hitResults, out int numHits)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 currentPos = projectile.predictedProjectilePosition.currentPos;
			Vector3 nextPos = projectile.predictedProjectilePosition.nextPos;
			Vector3 direction;
			float distance;
			WeaponRaycastUtility.Ray ray = CreateRay(currentPos, nextPos, out direction, out distance);
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.machineManager = networkMachineManager;
			parameters.fusionShieldTag = ((!projectile.projectileOwnerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
			parameters.shooterId = projectile.projectileOwnerComponent.ownerId;
			parameters.isShooterAi = projectile.projectileOwnerComponent.isAi;
			WeaponRaycastUtility.Parameters parameters2 = parameters;
			hitResults = new HitResult[projectile.weaponDamageComponent.hitDepth];
			return WeaponRaycastUtility.RaycastWeaponHit(ref ray, ref parameters2, hitResults, out numHits, projectile.weaponDamageComponent.hitDepth);
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

		private void CheckWeaponDamage(ProjectileNode projectile)
		{
			IWeaponDamageComponent weaponDamageComponent = projectile.weaponDamageComponent;
			int value = projectile.get_ID();
			weaponDamageComponent.weaponDamage.Dispatch(ref value);
		}

		private void HandleProjectileHit(ProjectileNode projectile, HitResult hitResult)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
			if (projectile.entitySourceComponent.isLocal)
			{
				TargetType targetType = hitResult.targetType;
				if (LayerToTargetType.IsTargetDestructible(targetType) && !hitResult.hitAlly && !hitResult.hitSelf && !hitResult.hitOwnBase)
				{
					int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitResult.hitTargetMachineId);
					bool targetIsMe = playerTeamsContainer.IsMe(targetType, playerFromMachineId);
					bool ownedByMe = projectileOwnerComponent.ownedByMe;
					Quaternion rotation = Quaternion.LookRotation(hitResult.normal);
					DetermineCubeHit(hitResult.hitPoint, rotation, hitResult.hitSelf, targetType, projectile, targetIsMe, ownedByMe);
				}
				else
				{
					Quaternion rotation2 = Quaternion.LookRotation(hitResult.normal);
					DeterminePropHit(ref hitResult, rotation2, targetType, projectile);
					_fireMissDependency.SetVariables(projectile.projectileOwnerComponent.machineId, projectile.itemDescriptorComponent.itemDescriptor, hitResult.hitPoint, hitResult.normal, _hit: true, hitResult.hitSelf || hitResult.hitOwnBase || hitResult.hitAlly, battleTimer.SecondsSinceGameInitialised, targetType);
					_broadcastMissClientCommand.Inject(_fireMissDependency).Execute();
				}
			}
		}

		private void DetermineCubeHit(Vector3 hitPosition, Quaternion rotation, bool hitSelf, TargetType targetType, ProjectileNode projectile, bool targetIsMe, bool shooterIsMe)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, hitSelf, hitPosition, rotation, Vector3.get_up(), targetIsMe, shooterIsMe);
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			switch (targetType)
			{
			case TargetType.Environment:
			case TargetType.FusionShield:
				break;
			case TargetType.Player:
				hitSomethingComponent.hitEnemy.Dispatch(ref value);
				break;
			case TargetType.TeamBase:
				hitSomethingComponent.hitProtonium.Dispatch(ref value);
				break;
			case TargetType.EqualizerCrystal:
				hitSomethingComponent.hitEqualizer.Dispatch(ref value);
				break;
			}
		}

		private void DeterminePropHit(ref HitResult hitResult, Quaternion rotation, TargetType targetType, ProjectileNode projectile)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, hitSelf_: false, hitResult.hitPoint, rotation, Vector3.get_up());
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			switch (targetType)
			{
			case TargetType.Player:
				if (hitResult.hitSelf || hitResult.hitAlly || hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				break;
			case TargetType.TeamBase:
			case TargetType.EqualizerCrystal:
				if (hitResult.hitOwnBase)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				break;
			case TargetType.FusionShield:
				hitSomethingComponent.hitFusionShield.Dispatch(ref value);
				break;
			case TargetType.Environment:
				hitSomethingComponent.hitEnvironment.Dispatch(ref value);
				break;
			}
		}
	}
}
