using Battle;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Laser
{
	internal sealed class LaserDamageEngine : SingleEntityViewEngine<LaserProjectileNode>, IInitialize
	{
		private Dictionary<int, LaserProjectileNode> _projectiles = new Dictionary<int, LaserProjectileNode>();

		private List<HitCubeInfo> _hitCubes = new List<HitCubeInfo>(32);

		private WeaponFireClientCommand _weaponFireClientCommand;

		private DestroyCubeDependency _weaponFireDependency = new DestroyCubeDependency();

		[Inject]
		public NetworkMachineManager networkMachineManager
		{
			get;
			private set;
		}

		[Inject]
		public RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			private set;
		}

		[Inject]
		public CubeDamagePropagator cubeDamagePropagator
		{
			get;
			private set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		public BattleTimer battleTimer
		{
			private get;
			set;
		}

		protected override void Add(LaserProjectileNode obj)
		{
			_projectiles.Add(obj.get_ID(), obj);
			obj.weaponDamageComponent.weaponDamage.subscribers += CheckWeaponDamage;
		}

		protected override void Remove(LaserProjectileNode obj)
		{
			_projectiles.Remove(obj.get_ID());
			obj.weaponDamageComponent.weaponDamage.subscribers -= CheckWeaponDamage;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireClientCommand = commandFactory.Build<WeaponFireClientCommand>();
		}

		private void CheckWeaponDamage(IWeaponDamageComponent sender, int id)
		{
			LaserProjectileNode projectile = _projectiles[id];
			HandleProjectileHit(projectile, sender.hitResults[0]);
		}

		private void HandleProjectileHit(LaserProjectileNode projectile, HitResult hitResult)
		{
			IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
			IEntitySourceComponent entitySourceComponent = projectile.entitySourceComponent;
			TargetType targetType = hitResult.targetType;
			if (!LayerToTargetType.IsTargetDestructible(targetType))
			{
				return;
			}
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitResult.hitTargetMachineId);
			if (rigidBodyData != null)
			{
				IMachineMap machineMap = networkMachineManager.GetMachineMap(targetType, hitResult.hitTargetMachineId);
				if (!hitResult.hitAlly && !hitResult.hitOwnBase && entitySourceComponent.isLocal && !hitResult.hitSelf)
				{
					DirectHit(hitResult, machineMap, projectile);
				}
			}
		}

		private void DirectHit(HitResult hitResult, IMachineMap hitMachineMap, LaserProjectileNode projectile)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Transform t = projectile.transformComponent.T;
			TargetType targetType = hitResult.targetType;
			List<HitCubeInfo> hitCubes = GenerateHitCubes(hitResult, hitMachineMap, projectile.projectileDamageStats);
			CreateFireCommand(hitResult.hitTargetMachineId, hitCubes, hitResult.hitPoint, hitResult.normal, targetType, t.get_position(), projectile);
		}

		private List<HitCubeInfo> GenerateHitCubes(HitResult hitResult, IMachineMap hitMachineMap, IProjectileDamageStatsComponent damageStats)
		{
			TargetType targetType = hitResult.targetType;
			_hitCubes.Clear();
			InstantiatedCube target = null;
			if (LayerToTargetType.IsTargetDestructible(targetType))
			{
				target = MachineUtility.GetCubeAtGridPosition(hitResult.gridHit.hitGridPos, hitMachineMap);
			}
			Dictionary<InstantiatedCube, int> proposedDamage = cubeDamagePropagator.GetProposedDamage(target, damageStats.damageBoost, damageStats.damage, damageStats.damageBuff, damageStats.damageMultiplier, damageStats.protoniumDamageScale, damageStats.campaignDifficultyFactor);
			cubeDamagePropagator.GenerateDestructionGroupHitInfo(proposedDamage, _hitCubes);
			return _hitCubes;
		}

		private void CreateFireCommand(int hitMachineId, List<HitCubeInfo> hitCubes, Vector3 hitPoint, Vector3 normal, TargetType targetType, Vector3 pos, LaserProjectileNode projectile)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (hitCubes.Count > 0)
			{
				HitCubeInfo hitCubeInfo = hitCubes[0];
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitPoint - cubeWorldPosition;
				int weaponDamage = CubeDamagePropagator.GetWeaponDamage(projectile.projectileDamageStats);
				_weaponFireDependency.SetVariables(projectile.projectileOwnerComponent.machineId, hitMachineId, hitEffectOffset, normal, projectile.itemDescriptorComponent.itemDescriptor, hitCubes, battleTimer.SecondsSinceGameInitialised, targetType, weaponDamage);
				_weaponFireClientCommand.Inject(_weaponFireDependency).Execute();
			}
		}
	}
}
