using Battle;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Nano
{
	internal sealed class HealingProjectileImpactEngine : SingleEntityViewEngine<HealingProjectileImpactNode>, IInitialize, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<InstantiatedCube, int> _proposedResult = new Dictionary<InstantiatedCube, int>();

		private List<HitCubeInfo> _healedCubes = new List<HitCubeInfo>(32);

		private HealAllyClientCommand _healingCommand;

		private HealedAllyCubesDependency _healdedCubesDependency = new HealedAllyCubesDependency();

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
		public CubeHealingPropagator cubeHealingPropagator
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

		void IInitialize.OnDependenciesInjected()
		{
			_healingCommand = commandFactory.Build<HealAllyClientCommand>();
		}

		protected override void Add(HealingProjectileImpactNode node)
		{
			node.weaponDamageComponent.weaponDamage.subscribers += CheckWeaponDamage;
		}

		protected override void Remove(HealingProjectileImpactNode node)
		{
			node.weaponDamageComponent.weaponDamage.subscribers -= CheckWeaponDamage;
		}

		private void CheckWeaponDamage(IWeaponDamageComponent sender, int id)
		{
			HealingProjectileImpactNode projectile = default(HealingProjectileImpactNode);
			if (entityViewsDB.TryQueryEntityView<HealingProjectileImpactNode>(id, ref projectile))
			{
				HitResult hitResult = sender.hitResults[0];
				HandleProjectileHit(projectile, hitResult);
			}
		}

		private void HandleProjectileHit(HealingProjectileImpactNode projectile, HitResult hitResult)
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
			IEntitySourceComponent entitySourceComponent = projectile.entitySourceComponent;
			bool didHeal = false;
			TargetType targetType = hitResult.targetType;
			if (hitResult.hitAlly && entitySourceComponent.isLocal && !hitResult.hitSelf && cubeHealingPropagator.PlayerCanBeHealed(targetType, hitResult.hitTargetMachineId) && HasPlayerHealingPriority(projectileOwnerComponent.machineId, hitResult.hitTargetMachineId))
			{
				IMachineMap machineMap = networkMachineManager.GetMachineMap(targetType, hitResult.hitTargetMachineId);
				InstantiatedCube cubeAtGridPosition = MachineUtility.GetCubeAtGridPosition(hitResult.gridHit.hitGridPos, machineMap);
				_healedCubes.Clear();
				DirectHitOnAlly(hitResult, projectile.projectileDamageStats, cubeAtGridPosition, _healedCubes, projectile);
				if (_healedCubes != null && _healedCubes.Count > 0)
				{
					Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitResult.hitTargetMachineId);
					HitCubeInfo hitCubeInfo = _healedCubes[0];
					Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
					Vector3 hitEffectOffset = hitResult.hitPoint - cubeWorldPosition;
					_healdedCubesDependency.SetVariables(hitResult.hitTargetMachineId, projectileOwnerComponent.machineId, projectileOwnerComponent.ownerId, _healedCubes, hitEffectOffset, hitResult.normal, projectile.itemDescriptorComponent.itemDescriptor.itemSize, TargetType.Player, battleTimer.SecondsSinceGameInitialised);
					_healingCommand.Inject(_healdedCubesDependency);
					_healingCommand.Execute();
					didHeal = true;
				}
			}
			ApplyImpactEffect(projectile, hitResult, didHeal);
		}

		private bool HasPlayerHealingPriority(int healerId, int hitTargetMachineId)
		{
			HealingPriorityNode healingPriorityNode = null;
			if (entityViewsDB.TryQueryEntityView<HealingPriorityNode>(hitTargetMachineId, ref healingPriorityNode))
			{
				int healerId2 = healingPriorityNode.healingPriorityComponent.healerId;
				return healerId2 == -1 || healerId2 == healerId;
			}
			return true;
		}

		private void DirectHitOnAlly(HitResult hitResult, IProjectileDamageStatsComponent damageStats, InstantiatedCube hitCubeInstance, List<HitCubeInfo> healedCubes, HealingProjectileImpactNode projectile)
		{
			_proposedResult.Clear();
			cubeHealingPropagator.ComputeProposedHeal(hitResult.hitTargetMachineId, hitCubeInstance, Mathf.CeilToInt(damageStats.campaignDifficultyFactor * damageStats.damageBoost * (float)damageStats.damage * damageStats.damageMultiplier * damageStats.damageBuff), ref _proposedResult);
			if (_proposedResult.Count > 0)
			{
				cubeHealingPropagator.GenerateHealingGroupHitInfo(_proposedResult, healedCubes);
			}
		}

		private void ApplyImpactEffect(HealingProjectileImpactNode projectile, HitResult hit, bool didHeal)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			IHitSomethingComponent hitSomethingComponent = projectile.hitSomethingComponent;
			HitInfo value = new HitInfo(hit.targetType, projectile.itemDescriptorComponent.itemDescriptor, !hit.hitAlly, hit_: true, hit.hitSelf, hit.hitPoint, Quaternion.get_identity(), hit.normal, hit.hitSelf, projectile.projectileOwnerComponent.ownedByMe, hit.hitAlly);
			if (value.targetType == TargetType.Player)
			{
				if (value.hitAlly && didHeal)
				{
					hitSomethingComponent.hitAlly.Dispatch(ref value);
				}
				else if (value.hitAlly && !didHeal && !hit.hitSelf)
				{
					hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				else
				{
					hitSomethingComponent.hitEnvironment.Dispatch(ref value);
				}
			}
			else
			{
				hitSomethingComponent.hitEnvironment.Dispatch(ref value);
			}
		}
	}
}
