using Battle;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamEngine : SingleEntityViewEngine<TeslaRamNode>, IInitialize, IPhysicallyTickable, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float RADIUS = 10f;

		private const float EARLY_OUT_FOR_DAMAGE_DISTANCE_SQ = 4f;

		private TeslaFireObservable _teslaFireObservable;

		private WeaponFiredObservable _weaponFiredObservable;

		private NetworkWeaponFiredObserver _networkWeaponFiredObserver;

		private GridAllignedLineCheck.GridAlignedCheckDependency _gridAlignedCheckDependency = new GridAllignedLineCheck.GridAlignedCheckDependency();

		private HitResult[] _hitResults = new HitResult[1];

		private List<HitCubeInfo> _destroyedCubes = new List<HitCubeInfo>(32);

		private WeaponFireClientCommand _weaponFireClientCommand;

		private DestroyCubeDependency _weaponFireDependency = new DestroyCubeDependency();

		private FasterList<TeslaRamNode> _teslaRamNodes = new FasterList<TeslaRamNode>(10);

		[Inject]
		internal CubeDamagePropagator cubeDamagePropagator
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
		internal RigidbodyDataContainer rigidbodyDataContainer
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
		internal LivePlayersContainer livePlayersContainer
		{
			get;
			private set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal CrosshairController crosshairController
		{
			get;
			set;
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

		public TeslaRamEngine(WeaponFiredObservable weaponFiredObservable, TeslaFireObservable teslaTeslaFireObservable, NetworkWeaponFiredObserver networkWeaponFiredObserver)
		{
			_teslaFireObservable = teslaTeslaFireObservable;
			_weaponFiredObservable = weaponFiredObservable;
			_networkWeaponFiredObserver = networkWeaponFiredObserver;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireClientCommand = commandFactory.Build<WeaponFireClientCommand>();
			destructionReporter.OnMachineKilled += OnMachineKilled;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= OnMachineKilled;
		}

		protected override void Add(TeslaRamNode node)
		{
			IHardwareOwnerComponent weaponOwnerComponent = node.weaponOwnerComponent;
			if (weaponOwnerComponent.ownedByMe || weaponOwnerComponent.ownedByAi)
			{
				node.shootingComponent.shotIsGoingToBeFired.subscribers += HandleShotGoingToBeFired;
				node.healthStatusComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnWeaponDestroyed);
				node.weaponActiveComponent.onActiveChanged.NotifyOnValueSet((Action<int, bool>)OnWeaponDestroyed);
				_teslaRamNodes.Add(node);
			}
		}

		protected override void Remove(TeslaRamNode node)
		{
			IHardwareOwnerComponent weaponOwnerComponent = node.weaponOwnerComponent;
			if (weaponOwnerComponent.ownedByMe || weaponOwnerComponent.ownedByAi)
			{
				node.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleShotGoingToBeFired;
				node.healthStatusComponent.isPartEnabled.StopNotify((Action<int, bool>)OnWeaponDestroyed);
				node.weaponActiveComponent.onActiveChanged.StopNotify((Action<int, bool>)OnWeaponDestroyed);
				_teslaRamNodes.UnorderedRemove(node);
			}
		}

		private void OnWeaponDestroyed(int weaponId, bool enabled)
		{
			TeslaRamNode teslaRamNode = default(TeslaRamNode);
			if (!enabled && entityViewsDB.TryQueryEntityView<TeslaRamNode>(weaponId, ref teslaRamNode))
			{
				ITeslaTargetComponent teslaTargetComponent = teslaRamNode.teslaTargetComponent;
				teslaTargetComponent.hasTarget = false;
				teslaTargetComponent.targetColliders.FastClear();
			}
		}

		public void PhysicsTick(float deltaSec)
		{
			for (int i = 0; i < _teslaRamNodes.get_Count(); i++)
			{
				TeslaRamNode teslaRamNode = _teslaRamNodes.get_Item(i);
				if (!teslaRamNode.weaponActiveComponent.active || !teslaRamNode.healthStatusComponent.enabled)
				{
					continue;
				}
				ITeslaTargetComponent teslaTargetComponent = teslaRamNode.teslaTargetComponent;
				RemoveInActiveColliders(teslaTargetComponent);
				if (teslaTargetComponent.hasTarget)
				{
					if (livePlayersContainer.IsPlayerAlive(teslaTargetComponent.targetType, teslaTargetComponent.playerId))
					{
						int machineId = teslaRamNode.weaponOwnerComponent.machineId;
						_teslaFireObservable.Dispatch(ref machineId);
					}
					else
					{
						teslaTargetComponent.hasTarget = false;
						teslaTargetComponent.targetColliders.FastClear();
					}
				}
			}
		}

		private void RemoveInActiveColliders(ITeslaTargetComponent target)
		{
			for (int i = 0; i < target.targetColliders.get_Count(); i++)
			{
				GameObject gameObject = target.targetColliders.get_Item(i).get_gameObject();
				if (gameObject.get_layer() == GameLayers.IGNORE_RAYCAST || !gameObject.get_activeInHierarchy())
				{
					target.targetColliders.UnorderedRemoveAt(i);
					i--;
				}
				if (target.targetColliders.get_Count() <= 0)
				{
					target.hasTarget = false;
				}
			}
		}

		private void HandleShotGoingToBeFired(IShootingComponent sender, int senderWeaponId)
		{
			FindWeaponToFire(senderWeaponId);
		}

		private void FindWeaponToFire(int weaponId)
		{
			TeslaRamNode teslaRamNode = default(TeslaRamNode);
			if (!entityViewsDB.TryQueryEntityView<TeslaRamNode>(weaponId, ref teslaRamNode))
			{
				return;
			}
			int num = default(int);
			TeslaRamNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<TeslaRamNode>(teslaRamNode.weaponOwnerComponent.machineId, ref num);
			int num2 = 0;
			TeslaRamNode teslaRamNode2;
			while (true)
			{
				if (num2 < num)
				{
					teslaRamNode2 = array[num2];
					if (teslaRamNode2.weaponActiveComponent.active && teslaRamNode2.teslaTargetComponent.hasTarget)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			FireWeapon(teslaRamNode2);
		}

		private void FireWeapon(TeslaRamNode weapon)
		{
			ITeslaTargetComponent teslaTargetComponent = weapon.teslaTargetComponent;
			if (livePlayersContainer.IsPlayerAlive(teslaTargetComponent.targetType, teslaTargetComponent.playerId))
			{
				HitTarget(weapon, teslaTargetComponent);
				return;
			}
			teslaTargetComponent.hasTarget = false;
			teslaTargetComponent.targetColliders.FastClear();
		}

		private void ApplyReactiveForce(TeslaRamNode node)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			FasterList<Collider> targetColliders = node.teslaTargetComponent.targetColliders;
			int num = 0;
			while (true)
			{
				if (num < node.teslaTargetComponent.targetColliders.get_Count())
				{
					if (targetColliders.get_Item(num).get_name().Equals("MechLegShootingOnlyCollider"))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Vector3 val = node.transformComponent.T.get_position() - targetColliders.get_Item(num).get_transform().get_position();
			Vector3 normalized = val.get_normalized();
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, node.weaponOwnerComponent.machineId);
			float num2 = Vector3.Dot(rigidBodyData.get_velocity(), normalized);
			rigidBodyData.AddForce(normalized * (0f - num2), 2);
		}

		private void HitTarget(TeslaRamNode teslaRam, ITeslaTargetComponent target)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = teslaRam.transformComponent.T.get_position();
			IHardwareOwnerComponent weaponOwnerComponent = teslaRam.weaponOwnerComponent;
			IMachineMap machineMap = networkMachineManager.GetMachineMap(target.targetType, target.machineId);
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(target.targetType, target.machineId);
			InstantiatedCube collidingCube = GetCollidingCube(position, rigidBodyData, machineMap, target.targetType);
			if (!(collidingCube == null))
			{
				DealDamage(teslaRam, target.machineId, collidingCube, target.hitPoint, target.targetType);
				ApplyReactiveForce(teslaRam);
				PerformHitEffects(teslaRam, position, Quaternion.get_identity(), target.targetType, target.playerId == weaponOwnerComponent.ownerId, weaponOwnerComponent.ownedByMe, rigidBodyData);
				if (weaponOwnerComponent.ownedByMe)
				{
					float weaponFireCost = teslaRam.weaponFireCostComponent.weaponFireCost;
					_weaponFiredObservable.Dispatch(ref weaponFireCost);
					crosshairController.ShowDamageEffect(0);
					teslaRam.robotShakeComponent.applyShake.set_value(teslaRam.get_ID());
					teslaRam.cameraShakeComponent.applyShake.set_value(teslaRam.get_ID());
				}
			}
		}

		private void DealDamage(TeslaRamNode teslaRam, int hitMachineId, InstantiatedCube hitCube, Vector3 hitPoint, TargetType targetType)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			IProjectileDamageStatsComponent damageStatsComponent = teslaRam.damageStatsComponent;
			Dictionary<InstantiatedCube, int> proposedDamage = cubeDamagePropagator.GetProposedDamage(hitCube, damageStatsComponent.damageBoost, damageStatsComponent.damage, damageStatsComponent.damageBuff, damageStatsComponent.damageMultiplier, damageStatsComponent.protoniumDamageScale, damageStatsComponent.campaignDifficultyFactor);
			_destroyedCubes.Clear();
			cubeDamagePropagator.GenerateDestructionGroupHitInfo(proposedDamage, _destroyedCubes);
			if (_destroyedCubes.Count > 0)
			{
				HitCubeInfo hitCubeInfo = _destroyedCubes[0];
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitPoint - cubeWorldPosition;
				int weaponDamage = CubeDamagePropagator.GetWeaponDamage(damageStatsComponent);
				_weaponFireDependency.SetVariables(teslaRam.weaponOwnerComponent.machineId, hitMachineId, hitEffectOffset, Vector3.get_zero(), teslaRam.itemDescriptorComponent.itemDescriptor, _destroyedCubes, battleTimer.SecondsSinceGameInitialised, targetType, weaponDamage);
				_weaponFireClientCommand.Inject(_weaponFireDependency).Execute();
			}
		}

		private InstantiatedCube GetCollidingCube(Vector3 weaponPosition, Rigidbody targetRB, IMachineMap hitMachineMap, TargetType targetType)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			InstantiatedCube instantiatedCube = null;
			float num = float.MaxValue;
			HashSet<InstantiatedCube> remainingCubes = hitMachineMap.GetRemainingCubes();
			HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				Vector3 val = GridScaleUtility.GridToWorld(current.gridPos, targetType);
				val = targetRB.get_rotation() * val;
				val += targetRB.get_position();
				Vector3 val2 = val - weaponPosition;
				float sqrMagnitude = val2.get_sqrMagnitude();
				if (instantiatedCube == null && sqrMagnitude < 100f)
				{
					instantiatedCube = current;
					num = sqrMagnitude;
				}
				else if (instantiatedCube != null && sqrMagnitude < num && sqrMagnitude < 100f)
				{
					instantiatedCube = current;
					num = sqrMagnitude;
					if (sqrMagnitude < 4f)
					{
						return instantiatedCube;
					}
				}
			}
			return instantiatedCube;
		}

		private InstantiatedCube GetCollidingCube(Vector3 weaponPosition, Vector3 hitPoint, Rigidbody targetRB, IMachineMap hitMachineMap, Vector3 direction, TargetType targetType)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			_gridAlignedCheckDependency.Populate(hitPoint, targetRB, weaponPosition, direction, 10f, hitMachineMap, targetType, null);
			if (GridAllignedLineCheck.GetCubeInGridStepLine(_gridAlignedCheckDependency, _hitResults) > 0)
			{
				return MachineUtility.GetCubeAtGridPosition(_hitResults[0].gridHit.hitGridPos, hitMachineMap);
			}
			return null;
		}

		private void PerformHitEffects(TeslaRamNode node, Vector3 hitPosition, Quaternion rotation, TargetType targetType, bool isTargetMe, bool isShooterMe, Rigidbody hitMachine)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(targetType, node.itemDescriptorComponent.itemDescriptor, node.weaponOwnerComponent.isEnemy, hit_: true, hitSelf_: false, hitPosition, rotation, Vector3.get_up(), isTargetMe, isShooterMe, Object.op_Implicit(hitMachine));
			switch (targetType)
			{
			case TargetType.TeamBase:
				node.hitSomethingComponent.hitProtonium.Dispatch(ref value);
				break;
			case TargetType.EqualizerCrystal:
				node.hitSomethingComponent.hitEqualizer.Dispatch(ref value);
				break;
			}
			node.hitSomethingComponent.hitEnemy.Dispatch(ref value);
		}

		private void OnMachineKilled(int killedId, int shooterId)
		{
			for (int i = 0; i < _teslaRamNodes.get_Count(); i++)
			{
				TeslaRamNode teslaRamNode = _teslaRamNodes.get_Item(i);
				ITeslaTargetComponent teslaTargetComponent = teslaRamNode.teslaTargetComponent;
				if (teslaTargetComponent.hasTarget && teslaTargetComponent.playerId == killedId)
				{
					teslaTargetComponent.hasTarget = false;
					teslaTargetComponent.targetColliders.FastClear();
				}
			}
		}

		public void Ready()
		{
		}
	}
}
