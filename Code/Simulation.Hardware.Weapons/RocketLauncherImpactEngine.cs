using Battle;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class RocketLauncherImpactEngine : SingleEntityViewEngine<RocketLauncherImpactNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private WeaponFireNoEffectClientCommand _weaponFireNoEffectClientCommand;

		private WeaponFireEffectOnlyClientCommand _weaponFireEffectOnlyClientCommand;

		private BroadcastMissClientCommand _broadcastMissClientCommand;

		private FireMissDependency _fireMissDependency = new FireMissDependency();

		private DestroyCubeNoEffectDependency _weaponFireNoEffectDependency = new DestroyCubeNoEffectDependency();

		private DestroyCubeEffectOnlyDependency _weaponFireEffectOnlyDependency = new DestroyCubeEffectOnlyDependency();

		private WeaponSplashDamageUtility.Parameters _splashCalculationParameters = default(WeaponSplashDamageUtility.Parameters);

		private List<HitCubeInfo> _destroyedPlayerCubes = new List<HitCubeInfo>();

		private Dictionary<InstantiatedCube, int> _proposedDestroyedCubes = new Dictionary<InstantiatedCube, int>(30);

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
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
		internal BattleTimer battleTimer
		{
			get;
			private set;
		}

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
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(RocketLauncherImpactNode node)
		{
			node.damageComponent.weaponDamage.subscribers += OnImpact;
		}

		protected override void Remove(RocketLauncherImpactNode node)
		{
			node.damageComponent.weaponDamage.subscribers -= OnImpact;
		}

		public void OnDependenciesInjected()
		{
			_weaponFireNoEffectClientCommand = commandFactory.Build<WeaponFireNoEffectClientCommand>();
			_weaponFireEffectOnlyClientCommand = commandFactory.Build<WeaponFireEffectOnlyClientCommand>();
			_broadcastMissClientCommand = commandFactory.Build<BroadcastMissClientCommand>();
			_splashCalculationParameters.machineManager = networkMachineManager;
			_splashCalculationParameters.machineRootContainer = machineRootContainer;
			_splashCalculationParameters.rigidbodyDataContainer = rigidbodyDataContainer;
			_splashCalculationParameters.playerMachinesContainer = playerMachinesContainer;
			_splashCalculationParameters.playerTeamsContainer = playerTeamsContainer;
		}

		private void OnImpact(IWeaponDamageComponent damageComponent, int id)
		{
			RocketLauncherImpactNode rocketLauncherImpactNode = entityViewsDB.QueryEntityView<RocketLauncherImpactNode>(id);
			if (rocketLauncherImpactNode != null && rocketLauncherImpactNode.entitySourceComponent.isLocal)
			{
				ApplyHit(rocketLauncherImpactNode, damageComponent.hitResults[0]);
			}
		}

		private void ApplyHit(RocketLauncherImpactNode projectile, HitResult hitResult)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			Vector3 hitPoint = hitResult.hitPoint;
			Vector3 normal = hitResult.normal;
			WeaponSplashDamageUtility.SplashParameters splashParameters = default(WeaponSplashDamageUtility.SplashParameters);
			splashParameters.position = hitPoint;
			splashParameters.radius = projectile.splashComponent.damageRadius;
			splashParameters.direction = projectile.transformComponent.T.get_forward();
			splashParameters.additionalHits = projectile.splashComponent.additionalHits;
			splashParameters.coneAngle = projectile.splashComponent.coneAngle;
			_splashCalculationParameters.projectileOwnerTeam = projectile.projectileOwnerComponent.ownerTeam;
			TargetType targetType = hitResult.targetType;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (targetType != TargetType.FusionShield && !hitResult.hitSelf)
			{
				WeaponSplashDamageUtility.HitCubesResultList hitCubesResultList = WeaponSplashDamageUtility.SplashDamageCubesList(ref splashParameters, ref _splashCalculationParameters, ref hitResult);
				flag2 = ApplyDamage(projectile, hitResult, hitCubesResultList.teamBases, TargetType.TeamBase);
				flag = ApplyDamage(projectile, hitResult, hitCubesResultList.playerMachines, TargetType.Player);
				flag3 = ApplyDamage(projectile, hitResult, hitCubesResultList.equalizers, TargetType.EqualizerCrystal);
			}
			if (flag || flag2 || flag3)
			{
				bool ownedByMe = projectile.projectileOwnerComponent.ownedByMe;
				bool isEnemy = projectile.projectileOwnerComponent.isEnemy;
				targetType = TargetType.TeamBase;
				if (flag3)
				{
					targetType = TargetType.EqualizerCrystal;
				}
				if (flag)
				{
					targetType = TargetType.Player;
				}
				HitInfo hitInfo = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, isEnemy, hit_: true, hitSelf_: false, hitResult.hitPoint, Quaternion.get_identity(), hitResult.normal, targetIsMe_: false, ownedByMe);
				ApplyImpactEffect(projectile, ref hitInfo, hitResult.hitOwnBase);
			}
			else
			{
				bool ownedByMe2 = projectile.projectileOwnerComponent.ownedByMe;
				bool hitAlly = hitResult.hitAlly;
				HitInfo hitInfo2 = new HitInfo(hitResult.targetType, projectile.itemDescriptorComponent.itemDescriptor, isEnemy_: false, hit_: true, hitResult.hitSelf, hitPoint, Quaternion.get_identity(), normal, targetIsMe_: false, ownedByMe2, hitAlly);
				ApplyImpactEffect(projectile, ref hitInfo2, hitResult.hitOwnBase);
				CreateMissCommand(projectile, hitPoint, normal, explosion: true, hitResult.hitSelf, hitResult.targetType);
			}
		}

		private bool ApplyDamage(RocketLauncherImpactNode projectile, HitResult hitResult, Dictionary<int, FasterList<InstantiatedCube>> targets, TargetType targetType)
		{
			bool result = false;
			bool flag = false;
			foreach (KeyValuePair<int, FasterList<InstantiatedCube>> target in targets)
			{
				int key = target.Key;
				FasterList<InstantiatedCube> value = target.Value;
				_destroyedPlayerCubes.Clear();
				ProcessHitCubes(value, projectile.projectileDamageStats);
				if (_destroyedPlayerCubes.Count > 0)
				{
					CreateFireCommandNoEffect(targetType, key, projectile);
					if (!flag)
					{
						CreateCubeDestroyEffectOnlyCommand(targetType, key, ref hitResult, projectile);
						flag = true;
					}
					result = true;
				}
			}
			return result;
		}

		private void ProcessHitCubes(FasterList<InstantiatedCube> hitCubes, IProjectileDamageStatsComponent damageStats)
		{
			_proposedDestroyedCubes.Clear();
			for (int num = hitCubes.get_Count() - 1; num >= 0; num--)
			{
				InstantiatedCube target = hitCubes.get_Item(num);
				int damage = Mathf.CeilToInt(damageStats.campaignDifficultyFactor * damageStats.damageBoost * damageStats.damageMultiplier * damageStats.damageBuff * (float)damageStats.damage / (float)hitCubes.get_Count());
				cubeDamagePropagator.ComputeProposedDamage(target, damage, damageStats.protoniumDamageScale, ref _proposedDestroyedCubes);
			}
			if (_proposedDestroyedCubes.Count > 0)
			{
				cubeDamagePropagator.GenerateDestructionGroupHitInfo(_proposedDestroyedCubes, _destroyedPlayerCubes);
			}
		}

		private void CreateFireCommandNoEffect(TargetType targetType, int hitTargetMachineId, RocketLauncherImpactNode projectile)
		{
			_weaponFireNoEffectDependency.SetVariables(projectile.projectileOwnerComponent.machineId, hitTargetMachineId, _destroyedPlayerCubes, battleTimer.SecondsSinceGameInitialised, targetType);
			_weaponFireNoEffectClientCommand.Inject(_weaponFireNoEffectDependency).Execute();
		}

		private void ApplyImpactEffect(RocketLauncherImpactNode projectile, ref HitInfo hitInfo, bool hitOwnBase)
		{
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			switch (hitInfo.targetType)
			{
			case TargetType.TeamBase:
				if (!hitOwnBase)
				{
					hitSomethingComponent.hitProtonium.Dispatch(ref hitInfo);
				}
				hitSomethingComponent.hitEnvironment.Dispatch(ref hitInfo);
				break;
			case TargetType.EqualizerCrystal:
				if (!hitOwnBase)
				{
					hitSomethingComponent.hitEqualizer.Dispatch(ref hitInfo);
				}
				hitSomethingComponent.hitEnvironment.Dispatch(ref hitInfo);
				break;
			case TargetType.FusionShield:
				hitSomethingComponent.hitFusionShield.Dispatch(ref hitInfo);
				break;
			case TargetType.Player:
				if (hitInfo.hitSelf)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref hitInfo);
				}
				else if (!hitInfo.hitAlly)
				{
					hitSomethingComponent.hitEnemy.Dispatch(ref hitInfo);
				}
				else
				{
					hitSomethingComponent.hitEnvironment.Dispatch(ref hitInfo);
				}
				break;
			default:
				hitSomethingComponent.hitEnvironment.Dispatch(ref hitInfo);
				break;
			}
		}

		private void CreateMissCommand(RocketLauncherImpactNode projectileStats, Vector3 position, Vector3 normal, bool explosion, bool hitSelf, TargetType targetType)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			_fireMissDependency.SetVariables(projectileStats.projectileOwnerComponent.machineId, projectileStats.itemDescriptorComponent.itemDescriptor, position, normal, explosion, hitSelf, battleTimer.SecondsSinceGameInitialised, targetType);
			_broadcastMissClientCommand.Inject(_fireMissDependency);
			_broadcastMissClientCommand.Execute();
		}

		private void CreateCubeDestroyEffectOnlyCommand(TargetType targetType, int hitTargetMachineId, ref HitResult hitResult, RocketLauncherImpactNode projectile)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (_destroyedPlayerCubes.Count > 0)
			{
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitTargetMachineId);
				HitCubeInfo hitCubeInfo = _destroyedPlayerCubes[0];
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitResult.hitPoint - cubeWorldPosition;
				DestroyCubeEffectOnlyDependency weaponFireEffectOnlyDependency = _weaponFireEffectOnlyDependency;
				HitCubeInfo hitCubeInfo2 = _destroyedPlayerCubes[0];
				weaponFireEffectOnlyDependency.SetVariables(hitCubeInfo2.gridLoc, projectile.projectileOwnerComponent.machineId, hitTargetMachineId, targetType, projectile.itemDescriptorComponent.itemDescriptor, hitEffectOffset, hitResult.normal);
				_weaponFireEffectOnlyClientCommand.Inject(_weaponFireEffectOnlyDependency);
				_weaponFireEffectOnlyClientCommand.Execute();
			}
		}
	}
}
