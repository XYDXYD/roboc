using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal sealed class AlliesFusionShieldEngine : SingleEntityViewEngine<FusionShieldEntityView>, IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const float SEND_RATE = 0.5f;

		private float _globalTimer;

		private uint _healPerSecond = 1000u;

		private Dictionary<InstantiatedCube, int> _proposedResult = new Dictionary<InstantiatedCube, int>();

		private List<HitCubeInfo> _healedCubes = new List<HitCubeInfo>();

		[Inject]
		internal CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool objectPool
		{
			private get;
			set;
		}

		[Inject]
		internal FusionShieldsObserver shieldsObserver
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
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerRespawnedIn += HandleOnPlayerRespawnedIn;
			shieldsObserver.RegisterShieldStateChanged(ShieldStateChanged);
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			LoadGameClientSettings();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			spawnDispatcher.OnPlayerRespawnedIn -= HandleOnPlayerRespawnedIn;
			shieldsObserver.UnregisterShieldStateChanged(ShieldStateChanged);
		}

		private void LoadGameClientSettings()
		{
			IGetGameClientSettingsRequest getGameClientSettingsRequest = serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_healPerSecond = data.fusionShieldHPS;
			}, delegate(ServiceBehaviour behaviour)
			{
				ErrorWindow.ShowServiceErrorWindow(behaviour);
			}));
			getGameClientSettingsRequest.Execute();
		}

		private void HandleOnPlayerRespawnedIn(SpawnInParametersPlayer spawnInParamaters)
		{
			CurableMachineNode curableMachineNode = default(CurableMachineNode);
			if (spawnInParamaters.isLocal && entityViewsDB.TryQueryEntityView<CurableMachineNode>(spawnInParamaters.machineId, ref curableMachineNode))
			{
				curableMachineNode.fusionShieldHealthChangeComponent.isHealing = IsInFriendlyShield(curableMachineNode);
			}
		}

		protected override void Add(FusionShieldEntityView entityView)
		{
			entityView.fusionShieldColliderComponent.machineBlockColliderEnabled = entityView.activableComponent.powerState;
		}

		protected override void Remove(FusionShieldEntityView entityView)
		{
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				_globalTimer += Time.get_deltaTime();
				FasterReadOnlyList<CurableMachineNode> curableMachines = entityViewsDB.QueryEntityViews<CurableMachineNode>();
				for (int i = 0; i < curableMachines.get_Count(); i++)
				{
					ProcessCurableMachine(curableMachines.get_Item(i));
				}
				if (_globalTimer > 0.5f)
				{
					ComputeAndSendData();
					_globalTimer = 0f;
				}
				yield return null;
			}
		}

		private void ProcessCurableMachine(CurableMachineNode curableMachine)
		{
			if (!curableMachine.aliveStateComponent.isAlive.get_value())
			{
				return;
			}
			if (IsInFriendlyShield(curableMachine))
			{
				if (!WasHealingInsideShield(curableMachine))
				{
					PlayerEnteredRange(curableMachine);
				}
				if (CanBeCured(curableMachine))
				{
					curableMachine.fusionShieldHealthChangeComponent.timeHealthChanging += Time.get_deltaTime();
				}
			}
			else if (WasHealingInsideShield(curableMachine))
			{
				PlayerExitedRange(curableMachine);
			}
		}

		private void ShieldStateChanged(int teamId, bool fullPower)
		{
			FusionShieldEntityView fusionShieldEntityView = default(FusionShieldEntityView);
			if (entityViewsDB.TryQueryEntityView<FusionShieldEntityView>(teamId, ref fusionShieldEntityView))
			{
				fusionShieldEntityView.fusionShieldColliderComponent.machineBlockColliderEnabled = fullPower;
			}
		}

		private void PlayerExitedRange(CurableMachineNode curableMachine)
		{
			curableMachine.fusionShieldHealthChangeComponent.isHealing = false;
			curableMachine.fusionShieldHealthChangeComponent.timeHealthChanging = 0f;
			GameObject audioGameObject = GetAudioGameObject(curableMachine);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.FusionShieldExit), audioGameObject);
		}

		private void PlayerEnteredRange(CurableMachineNode curableMachine)
		{
			curableMachine.fusionShieldHealthChangeComponent.isHealing = true;
			GameObject audioGameObject = GetAudioGameObject(curableMachine);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.FusionShieldEnter), audioGameObject);
		}

		private GameObject GetAudioGameObject(CurableMachineNode curableMachine)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = objectPool.Use(1, (Func<GameObject>)objectPool.AddRecycleOnDisableForAudio);
			val.SetActive(true);
			val.get_transform().set_position(curableMachine.rigidbodyComponent.rb.get_worldCenterOfMass());
			return val;
		}

		private bool CanBeCured(CurableMachineNode curableMachine)
		{
			int ownerId = curableMachine.machineOwnerComponent.ownerId;
			int iD = curableMachine.get_ID();
			return curableMachine.aliveStateComponent.isAlive.get_value() && healingPropagator.PlayerCanBeHealed(TargetType.Player, iD);
		}

		private bool IsInFriendlyShield(CurableMachineNode curableMachine)
		{
			return curableMachine.insideFusionShieldComponent.isInsideShield && curableMachine.insideFusionShieldComponent.teamId == curableMachine.ownerTeamComponent.ownerTeamId;
		}

		private bool WasHealingInsideShield(CurableMachineNode curableMachine)
		{
			return curableMachine.fusionShieldHealthChangeComponent.isHealing && curableMachine.insideFusionShieldComponent.teamId == curableMachine.ownerTeamComponent.ownerTeamId;
		}

		private void ComputeAndSendData()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CurableMachineNode> val = entityViewsDB.QueryEntityViews<CurableMachineNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				CurableMachineNode curableMachineNode = val.get_Item(i);
				if (curableMachineNode.fusionShieldHealthChangeComponent.isHealing)
				{
					ResetData();
					ComputeHealedCubes(curableMachineNode);
					SendDataToServer(curableMachineNode.machineOwnerComponent.ownerMachineId);
					curableMachineNode.fusionShieldHealthChangeComponent.timeHealthChanging = 0f;
				}
			}
		}

		private void ComputeHealedCubes(CurableMachineNode curableMachine)
		{
			int ownerMachineId = curableMachine.machineOwnerComponent.ownerMachineId;
			int heal = Mathf.CeilToInt((float)(double)_healPerSecond * curableMachine.fusionShieldHealthChangeComponent.timeHealthChanging);
			healingPropagator.ComputeProposedHeal(TargetType.Player, ownerMachineId, heal, ref _proposedResult);
			if (_proposedResult.Count > 0)
			{
				healingPropagator.GenerateHealingGroupHitInfo(_proposedResult, _healedCubes);
			}
		}

		private void SendDataToServer(int machineId)
		{
			HealedCubesDependency dependency = new HealedCubesDependency(machineId, _healedCubes, TargetType.FusionShield, TargetType.Player);
			HealSelfCommand healSelfCommand = commandFactory.Build<HealSelfCommand>();
			healSelfCommand.Inject(dependency);
			healSelfCommand.Execute();
		}

		private void ResetData()
		{
			_proposedResult.Clear();
			_healedCubes.Clear();
		}

		public void Ready()
		{
		}
	}
}
