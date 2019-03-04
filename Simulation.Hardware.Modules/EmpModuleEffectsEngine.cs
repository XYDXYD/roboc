using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleEffectsEngine : MultiEntityViewsEngine<EmpLocatorEffectsNode, EmpCameraEffectsNode, MachineStunNode>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private GameObject _currentEffectToInstantiate;

		private Func<GameObject> _OnMachineEffectFirstAllocation;

		private NetworkStunMachineObserver _empMachineEffectObserver;

		private EmpLocatorEffectsNode _effectsNode;

		private EmpCameraEffectsNode _cameraEffectsNode;

		private string _qualityLevelName;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal EmpMainBeamFactory empMainBeamFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CrackDecalProjectorFactory crackDecalProjectorFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe EmpModuleEffectsEngine(NetworkStunMachineObserver observer)
		{
			_empMachineEffectObserver = observer;
			_empMachineEffectObserver.AddAction(new ObserverAction<NetworkStunnedMachineData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			int qualityLevel = QualitySettings.GetQualityLevel();
			_qualityLevelName = QualitySettings.get_names()[qualityLevel];
		}

		public void Ready()
		{
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_empMachineEffectObserver.RemoveAction(new ObserverAction<NetworkStunnedMachineData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void PreallocateEffects(EmpLocatorEffectsNode effectsNode)
		{
			GameObject stunMainEffectAllyPrefab = effectsNode.effectsComponent.stunMainEffectAllyPrefab;
			GameObject stunMainEffectEnemyPrefab = effectsNode.effectsComponent.stunMainEffectEnemyPrefab;
			GameObject machineStunEffectAllyPrefab = effectsNode.effectsComponent.machineStunEffectAllyPrefab;
			GameObject machineStunEffectEnemyPrefab = effectsNode.effectsComponent.machineStunEffectEnemyPrefab;
			GameObject machineRecoverEffectAllyPrefab = effectsNode.effectsComponent.machineRecoverEffectAllyPrefab;
			GameObject machineRecoverEffectEnemyPrefab = effectsNode.effectsComponent.machineRecoverEffectEnemyPrefab;
			GameObject crackDecalAllyPrefab = effectsNode.effectsComponent.crackDecalAllyPrefab;
			GameObject crackDecalEnemyPrefab = effectsNode.effectsComponent.crackDecalEnemyPrefab;
			GameObject glowFloorEffectAllyPrefab = effectsNode.effectsComponent.glowFloorEffectAllyPrefab;
			GameObject glowFloorEffectEnemyPrefab = effectsNode.effectsComponent.glowFloorEffectEnemyPrefab;
			_OnMachineEffectFirstAllocation = OnMachineEffectFirstAllocation;
			empMainBeamFactory.PreallocateMainBeam(stunMainEffectAllyPrefab, 1);
			empMainBeamFactory.PreallocateMainBeam(stunMainEffectEnemyPrefab, 1);
			crackDecalProjectorFactory.PreallocateCrackDecal(crackDecalAllyPrefab, 1);
			crackDecalProjectorFactory.PreallocateCrackDecal(crackDecalEnemyPrefab, 1);
			_currentEffectToInstantiate = machineStunEffectAllyPrefab;
			gameObjectPool.Preallocate(machineStunEffectAllyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
			_currentEffectToInstantiate = machineStunEffectEnemyPrefab;
			gameObjectPool.Preallocate(machineStunEffectEnemyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
			_currentEffectToInstantiate = machineRecoverEffectAllyPrefab;
			gameObjectPool.Preallocate(machineRecoverEffectAllyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
			_currentEffectToInstantiate = machineRecoverEffectEnemyPrefab;
			gameObjectPool.Preallocate(machineRecoverEffectEnemyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
			_currentEffectToInstantiate = glowFloorEffectAllyPrefab;
			gameObjectPool.Preallocate(glowFloorEffectAllyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
			_currentEffectToInstantiate = glowFloorEffectEnemyPrefab;
			gameObjectPool.Preallocate(glowFloorEffectEnemyPrefab.get_name(), 1, _OnMachineEffectFirstAllocation);
		}

		private void HandlePlayMainStunEffect(IEmpStunActivationComponent sender, int locatorId)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			EmpLocatorEffectsNode empLocatorEffectsNode = default(EmpLocatorEffectsNode);
			if (entityViewsDB.TryQueryEntityView<EmpLocatorEffectsNode>(locatorId, ref empLocatorEffectsNode))
			{
				IEmpLocatorEffectsComponent effectsComponent = empLocatorEffectsNode.effectsComponent;
				GameObject prefab;
				if (empLocatorEffectsNode.ownerComponent.isOnMyTeam)
				{
					_currentEffectToInstantiate = effectsComponent.stunMainEffectAllyPrefab;
					prefab = effectsComponent.crackDecalAllyPrefab;
				}
				else
				{
					_currentEffectToInstantiate = effectsComponent.stunMainEffectEnemyPrefab;
					prefab = effectsComponent.crackDecalEnemyPrefab;
				}
				Vector3 position = empLocatorEffectsNode.transformComponent.empLocatorTransform.get_position();
				Vector3 localScale = empLocatorEffectsNode.transformComponent.empLocatorTransform.get_localScale();
				float stunDuration = empLocatorEffectsNode.stunDurationComponent.stunDuration;
				empMainBeamFactory.Build(_currentEffectToInstantiate, position, stunDuration, localScale);
				crackDecalProjectorFactory.Build(prefab, position, localScale, stunDuration);
			}
		}

		private void HandlePlayGlowFloorEffect(IEmpLocatorEffectsComponent sender, GlowFloorEffectData data)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			int locatorId = data.locatorId;
			Vector3 position = data.position;
			EmpLocatorEffectsNode empLocatorEffectsNode = default(EmpLocatorEffectsNode);
			if (entityViewsDB.TryQueryEntityView<EmpLocatorEffectsNode>(locatorId, ref empLocatorEffectsNode))
			{
				IEmpLocatorEffectsComponent effectsComponent = empLocatorEffectsNode.effectsComponent;
				if (empLocatorEffectsNode.ownerComponent.isOnMyTeam)
				{
					_currentEffectToInstantiate = effectsComponent.glowFloorEffectAllyPrefab;
				}
				else
				{
					_currentEffectToInstantiate = effectsComponent.glowFloorEffectEnemyPrefab;
				}
				GameObject val = gameObjectPool.Use(_currentEffectToInstantiate.get_name(), _OnMachineEffectFirstAllocation);
				val.get_transform().set_position(position);
				val.get_transform().set_localScale(empLocatorEffectsNode.transformComponent.empLocatorTransform.get_localScale());
				val.SetActive(true);
			}
		}

		private void HandlePlayMachineStunEffect(IMachineStunComponent sender, int machineId)
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			MachineStunNode machineStunNode = default(MachineStunNode);
			EmpLocatorEffectsNode empLocatorEffectsNode = default(EmpLocatorEffectsNode);
			if (sender.stunned && entityViewsDB.TryQueryEntityView<EmpLocatorEffectsNode>(sender.stunningEmpLocator, ref empLocatorEffectsNode) && entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode))
			{
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
				if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId))
				{
					_currentEffectToInstantiate = empLocatorEffectsNode.effectsComponent.machineStunEffectAllyPrefab;
				}
				else
				{
					_currentEffectToInstantiate = empLocatorEffectsNode.effectsComponent.machineStunEffectEnemyPrefab;
				}
				GameObject val = gameObjectPool.Use(_currentEffectToInstantiate.get_name(), _OnMachineEffectFirstAllocation);
				val.get_transform().set_parent(machineStunNode.rigidbodyComponent.rb.get_transform());
				val.get_transform().set_localPosition(machineStunNode.rigidbodyComponent.rb.get_centerOfMass());
				val.SetActive(true);
			}
		}

		private void HandlePlayMachineRecoverEffect(IMachineStunComponent sender, int machineId)
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			MachineStunNode machineStunNode = default(MachineStunNode);
			EmpLocatorEffectsNode empLocatorEffectsNode = default(EmpLocatorEffectsNode);
			if (!sender.stunned && entityViewsDB.TryQueryEntityView<EmpLocatorEffectsNode>(sender.stunningEmpLocator, ref empLocatorEffectsNode) && entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode))
			{
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
				if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId))
				{
					_currentEffectToInstantiate = empLocatorEffectsNode.effectsComponent.machineRecoverEffectAllyPrefab;
				}
				else
				{
					_currentEffectToInstantiate = empLocatorEffectsNode.effectsComponent.machineRecoverEffectEnemyPrefab;
				}
				GameObject val = gameObjectPool.Use(_currentEffectToInstantiate.get_name(), _OnMachineEffectFirstAllocation);
				val.get_transform().set_parent(machineStunNode.rigidbodyComponent.rb.get_transform());
				val.get_transform().set_localPosition(machineStunNode.rigidbodyComponent.rb.get_centerOfMass());
				val.SetActive(true);
			}
		}

		private void SpawnStunMachineEffect(ref NetworkStunnedMachineData data)
		{
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			MachineRigidbodyNode machineRigidbodyNode = default(MachineRigidbodyNode);
			if (!entityViewsDB.TryQueryEntityView<MachineRigidbodyNode>(data.machineId, ref machineRigidbodyNode))
			{
				return;
			}
			EmpLocatorEffectsNode effectsNode = _effectsNode;
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, data.machineId);
			bool flag = playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId);
			if (!data.isStunned)
			{
				if (flag)
				{
					_currentEffectToInstantiate = effectsNode.effectsComponent.machineRecoverEffectAllyPrefab;
				}
				else
				{
					_currentEffectToInstantiate = effectsNode.effectsComponent.machineRecoverEffectEnemyPrefab;
				}
			}
			else if (flag)
			{
				_currentEffectToInstantiate = effectsNode.effectsComponent.machineStunEffectAllyPrefab;
			}
			else
			{
				_currentEffectToInstantiate = effectsNode.effectsComponent.machineStunEffectEnemyPrefab;
			}
			GameObject val = gameObjectPool.Use(_currentEffectToInstantiate.get_name(), _OnMachineEffectFirstAllocation);
			val.get_transform().set_parent(machineRigidbodyNode.rigidbodyComponent.rb.get_transform());
			val.get_transform().set_localPosition(machineRigidbodyNode.rigidbodyComponent.rb.get_centerOfMass());
			val.SetActive(true);
		}

		private void HandlePlayCameraEmpEffects(IMachineStunComponent sender, int machineId)
		{
			MachineStunNode machineStunNode = default(MachineStunNode);
			if (entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode) && machineStunNode.ownerComponent.ownedByMe && _qualityLevelName != "Fastest" && _qualityLevelName != "Fast" && _qualityLevelName != "Normal")
			{
				_cameraEffectsNode.empEffectsComponent.enableEffectsScripts = sender.stunned;
				_cameraEffectsNode.empEffectsComponent.cameraStunSoundObject.SetActive(sender.stunned);
			}
		}

		private GameObject OnMachineEffectFirstAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentEffectToInstantiate);
		}

		protected override void Add(EmpLocatorEffectsNode effectsNode)
		{
			effectsNode.activationComponent.activateEmpStun.subscribers += HandlePlayMainStunEffect;
			effectsNode.effectsComponent.playGlowFloorEffect.subscribers += HandlePlayGlowFloorEffect;
			PreallocateEffects(effectsNode);
			_effectsNode = effectsNode;
		}

		protected override void Remove(EmpLocatorEffectsNode effectsNode)
		{
			effectsNode.activationComponent.activateEmpStun.subscribers -= HandlePlayMainStunEffect;
			effectsNode.effectsComponent.playGlowFloorEffect.subscribers -= HandlePlayGlowFloorEffect;
		}

		protected override void Add(EmpCameraEffectsNode effectsNode)
		{
			_cameraEffectsNode = effectsNode;
		}

		protected override void Remove(EmpCameraEffectsNode entityView)
		{
			_cameraEffectsNode = null;
		}

		protected override void Add(MachineStunNode machineStunNode)
		{
			machineStunNode.stunComponent.machineStunned.subscribers += HandlePlayMachineStunEffect;
			machineStunNode.stunComponent.machineStunned.subscribers += HandlePlayMachineRecoverEffect;
			machineStunNode.stunComponent.machineStunned.subscribers += HandlePlayCameraEmpEffects;
		}

		protected override void Remove(MachineStunNode machineStunNode)
		{
			machineStunNode.stunComponent.machineStunned.subscribers -= HandlePlayMachineStunEffect;
			machineStunNode.stunComponent.machineStunned.subscribers -= HandlePlayMachineRecoverEffect;
			machineStunNode.stunComponent.machineStunned.subscribers -= HandlePlayCameraEmpEffects;
		}
	}
}
