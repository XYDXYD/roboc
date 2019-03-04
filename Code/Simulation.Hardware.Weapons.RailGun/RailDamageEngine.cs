using Battle;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailDamageEngine : SingleEntityViewEngine<RailProjectileNode>, IInitialize
	{
		private Dictionary<InstantiatedCube, int> _proposedDestroyedCubes = new Dictionary<InstantiatedCube, int>();

		private Dictionary<int, RailProjectileNode> _projectiles = new Dictionary<int, RailProjectileNode>();

		private WeaponFireClientCommand _weaponFireClientCommand;

		private DestroyCubeDependency _weaponFireDependency = new DestroyCubeDependency();

		private List<HitCubeInfo> _hitCubes = new List<HitCubeInfo>();

		private const int NUM_DAMAGE_SECTIONS = 3;

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

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireClientCommand = commandFactory.Build<WeaponFireClientCommand>();
		}

		protected override void Add(RailProjectileNode obj)
		{
			_projectiles.Add(obj.get_ID(), obj);
			obj.weaponDamageComponent.weaponDamage.subscribers += CheckWeaponDamage;
		}

		protected override void Remove(RailProjectileNode obj)
		{
			_projectiles.Remove(obj.get_ID());
			obj.weaponDamageComponent.weaponDamage.subscribers -= CheckWeaponDamage;
		}

		private void CheckWeaponDamage(IWeaponDamageComponent sender, int id)
		{
			RailProjectileNode projectile = _projectiles[id];
			HandleProjectileHit(projectile, sender.hitResults, sender.numHits);
		}

		private void HandleProjectileHit(RailProjectileNode projectile, HitResult[] hitResults, int numHits)
		{
			IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
			IEntitySourceComponent entitySourceComponent = projectile.entitySourceComponent;
			HitResult hitResult = hitResults[0];
			TargetType targetType = hitResult.targetType;
			int hitTargetMachineId = hitResult.hitTargetMachineId;
			if (!LayerToTargetType.IsTargetDestructible(targetType))
			{
				return;
			}
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitTargetMachineId);
			if (rigidBodyData != null)
			{
				IMachineMap machineMap = networkMachineManager.GetMachineMap(targetType, hitTargetMachineId);
				if (!hitResult.hitAlly && !hitResult.hitSelf && !hitResult.hitOwnBase && entitySourceComponent.isLocal)
				{
					CalculateDamageOfHitCubes(hitTargetMachineId, hitResults, numHits, machineMap, projectile);
				}
			}
		}

		private void CalculateDamageOfHitCubes(int hitMachineId, HitResult[] hitResults, int numHits, IMachineMap hitMachineMap, RailProjectileNode projectile)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			List<HitCubeInfo> hitCubes = GenerateHitCubes(hitResults, numHits, hitMachineMap, projectile.projectileDamageStats);
			HitResult hitResult = hitResults[0];
			CreateFireCommand(hitMachineId, hitCubes, hitResult.hitPoint, hitResult.normal, hitResult.targetType, projectile.transformComponent.T.get_position(), projectile);
		}

		private List<HitCubeInfo> GenerateHitCubes(HitResult[] hitResults, int numHits, IMachineMap hitMachineMap, IProjectileDamageStatsComponent damageStats)
		{
			_proposedDestroyedCubes.Clear();
			HitResult hitResult = hitResults[0];
			int hitTargetMachineId = hitResult.hitTargetMachineId;
			TargetType targetType = hitResult.targetType;
			int num = Mathf.CeilToInt(damageStats.campaignDifficultyFactor * damageStats.damageBoost * damageStats.damageBuff * damageStats.damageMultiplier * (float)damageStats.damage);
			int num2 = num / 3;
			int num3 = 1;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			while (num7 < numHits && num3 <= 3)
			{
				HitResult hitResult2 = hitResults[num7];
				InstantiatedCube instantiatedCube = null;
				if (LayerToTargetType.IsTargetDestructible(targetType))
				{
					instantiatedCube = MachineUtility.GetCubeAtGridPosition(hitResult2.gridHit.hitGridPos, hitMachineMap);
				}
				if (num7 == numHits - 1)
				{
					num6 = num - num4;
				}
				else
				{
					if (_proposedDestroyedCubes.TryGetValue(instantiatedCube, out int value) && value <= 0)
					{
						num7++;
						continue;
					}
					num6 = ((num3 >= 3) ? (num - num2 * 2) : num2);
					num6 += num5;
				}
				int num8 = cubeDamagePropagator.ComputeProposedDamage(instantiatedCube, num6, damageStats.protoniumDamageScale, ref _proposedDestroyedCubes);
				num5 = num6 - num8;
				num4 += num8;
				num3++;
			}
			_hitCubes.Clear();
			cubeDamagePropagator.GenerateDestructionGroupHitInfo(_proposedDestroyedCubes, _hitCubes);
			return _hitCubes;
		}

		private void CreateFireCommand(int hitMachineId, List<HitCubeInfo> hitCubes, Vector3 hitPoint, Vector3 normal, TargetType targetType, Vector3 pos, RailProjectileNode projectile)
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
