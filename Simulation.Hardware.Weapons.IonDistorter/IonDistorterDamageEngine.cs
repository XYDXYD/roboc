using Battle;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterDamageEngine : SingleEntityViewEngine<IonDistorterProjectileNode>, IInitialize, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<uint, Dictionary<InstantiatedCube, int>> _proposedDamageResults = new Dictionary<uint, Dictionary<InstantiatedCube, int>>();

		private List<HitCubeInfo> _destroyedCubes = new List<HitCubeInfo>();

		private Dictionary<uint, HitData> _hitDataPerMachine = new Dictionary<uint, HitData>();

		private WeaponFireClientCommand _weaponFireClientCommand;

		private DestroyCubeDependency _weaponFireDependency = new DestroyCubeDependency();

		[Inject]
		internal RigidbodyDataContainer rigidBodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal CubeDamagePropagator cubeDamagePropagator
		{
			private get;
			set;
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

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireClientCommand = commandFactory.Build<WeaponFireClientCommand>();
		}

		protected override void Add(IonDistorterProjectileNode node)
		{
			node.weaponDamageComponent.weaponDamage.subscribers += ApplyIonDistorterDamage;
		}

		protected override void Remove(IonDistorterProjectileNode node)
		{
			node.weaponDamageComponent.weaponDamage.subscribers -= ApplyIonDistorterDamage;
		}

		private void ApplyIonDistorterDamage(IWeaponDamageComponent damageComponent, int projectileId)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterProjectileNode ionDistorterProjectileNode = default(IonDistorterProjectileNode);
			if (entityViewsDB.TryQueryEntityView<IonDistorterProjectileNode>(projectileId, ref ionDistorterProjectileNode) && ionDistorterProjectileNode.entitySourceComponent.isLocal)
			{
				_proposedDamageResults.Clear();
				_hitDataPerMachine.Clear();
				Vector3 position = ionDistorterProjectileNode.transformComponent.T.get_position();
				HitResult[] hitResults = damageComponent.hitResults;
				for (int i = 0; i < damageComponent.numHits; i++)
				{
					HitResult currentHit = hitResults[i];
					ApplyDamageToCurrentHit(ionDistorterProjectileNode, position, currentHit);
				}
				SendDamageToServer(ionDistorterProjectileNode);
			}
		}

		private void SendDamageToServer(IonDistorterProjectileNode projectile)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<uint, Dictionary<InstantiatedCube, int>>.Enumerator enumerator = _proposedDamageResults.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<InstantiatedCube, int> value = enumerator.Current.Value;
				uint key = enumerator.Current.Key;
				HitData hitData = _hitDataPerMachine[key];
				if (value.Count > 0)
				{
					_destroyedCubes.Clear();
					cubeDamagePropagator.GenerateDestructionGroupHitInfo(value, _destroyedCubes);
					HitCubeInfo hitCubeInfo = _destroyedCubes[0];
					Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, hitData.rb, hitData.type);
					Vector3 hitEffectOffset = hitData.hitPoint - cubeWorldPosition;
					int weaponDamage = CubeDamagePropagator.GetWeaponDamage(projectile.projectileDamageStatsComponent);
					if (_destroyedCubes.Count > 0)
					{
						_weaponFireDependency.SetVariables(projectile.projectileOwnerComponent.machineId, hitData.machineId, hitEffectOffset, hitData.normal, projectile.itemDescriptorComponent.itemDescriptor, _destroyedCubes, battleTimer.SecondsSinceGameInitialised, hitData.type, weaponDamage);
						_weaponFireClientCommand.Inject(_weaponFireDependency).Execute();
					}
				}
			}
		}

		private void ApplyDamageToCurrentHit(IonDistorterProjectileNode projectile, Vector3 projectilePosition, HitResult currentHit)
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			TargetType targetType = currentHit.targetType;
			int hitTargetMachineId = currentHit.hitTargetMachineId;
			int damage = Mathf.CeilToInt(projectile.projectileDamageStatsComponent.campaignDifficultyFactor * projectile.projectileDamageStatsComponent.damageBoost * projectile.projectileDamageStatsComponent.damageBuff * projectile.projectileDamageStatsComponent.damageMultiplier * (float)projectile.projectileDamageStatsComponent.damage / (float)projectile.coneComponent.numOfRaycasts);
			if (machineManager.IsPlayerMapRegisterred(currentHit.targetType, hitTargetMachineId))
			{
				IMachineMap machineMap = machineManager.GetMachineMap(currentHit.targetType, hitTargetMachineId);
				InstantiatedCube cubeAtGridPosition = MachineUtility.GetCubeAtGridPosition(currentHit.gridHit.hitGridPos, machineMap);
				uint key = PackId(hitTargetMachineId, targetType);
				if (!_proposedDamageResults.TryGetValue(key, out Dictionary<InstantiatedCube, int> value))
				{
					value = new Dictionary<InstantiatedCube, int>();
					_proposedDamageResults.Add(key, value);
					Vector3 hitPoint = currentHit.hitPoint;
					Vector3 normal = currentHit.normal;
					Rigidbody rigidBodyData = rigidBodyDataContainer.GetRigidBodyData(targetType, hitTargetMachineId);
					_hitDataPerMachine.Add(key, new HitData(targetType, hitPoint, normal, rigidBodyData, hitTargetMachineId));
				}
				cubeDamagePropagator.ComputeProposedDamage(cubeAtGridPosition, damage, projectile.projectileDamageStatsComponent.protoniumDamageScale, ref value);
			}
		}

		private uint PackId(int currentHitMachineId, TargetType type)
		{
			return (uint)(currentHitMachineId | ((int)type << 16));
		}

		public void Ready()
		{
		}
	}
}
