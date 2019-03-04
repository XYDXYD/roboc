using LobbyServiceLayer;
using RCNetwork.Events;
using RCNetwork.Server;
using Simulation.SinglePlayer.Spawn;
using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal sealed class CampaignSpawnPointsEngine : SingleEntityViewEngine<CurrentWaveTrackerEntityView>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private readonly AllowMovementObservable _allowMovementObserverable;

		private readonly PlayerSpawnPointObserver _playerSpawnPointObserver;

		private readonly AllPlayersSpawnedObservable _allPlayersSpawnedObservable;

		private readonly ICursorMode _cursorMode;

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			get;
			set;
		}

		[Inject]
		public ISpawnPointManager spawnPointManager
		{
			private get;
			set;
		}

		[Inject]
		public MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignSpawnPointsEngine(PlayerSpawnPointObserver playerSpawnPointObserver, AllPlayersSpawnedObservable allPlayerSpawnObservable, ICursorMode cursorMode, AllowMovementObservable allowMovementObservable)
		{
			_allPlayersSpawnedObservable = allPlayerSpawnObservable;
			_playerSpawnPointObserver = playerSpawnPointObserver;
			_playerSpawnPointObserver.AddAction((Action)HandleHumanPlayerRespawnPointRequest);
			_allowMovementObserverable = allowMovementObservable;
			_cursorMode = cursorMode;
		}

		void IQueryingEntityViewEngine.Ready()
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_playerSpawnPointObserver.RemoveAction((Action)HandleHumanPlayerRespawnPointRequest);
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.NotifyOnValueSet((Action<int, bool>)HandleNewWaveReady);
			entityView.CurrentWaveCounterComponent.counterValue.NotifyOnValueSet((Action<int, int>)HandleNewWave);
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.StopNotify((Action<int, bool>)HandleNewWaveReady);
			entityView.CurrentWaveCounterComponent.counterValue.StopNotify((Action<int, int>)HandleNewWave);
		}

		private void HandleHumanPlayerRespawnPointRequest()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			int playerId = entityViewsDB.QueryEntityViews<PlayerTargetNode>().get_Item(0).playerTargetGameObjectComponent.playerId;
			SpawningPoint nextFreeSpawnPoint = spawnPointManager.GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType.PitModeStartLocations, playerId);
			SpawnPointDependency dependency = new SpawnPointDependency(nextFreeSpawnPoint.get_transform().get_position(), nextFreeSpawnPoint.get_transform().get_rotation(), playerId);
			networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeRespawnPoint, playerId, dependency);
		}

		private void HandleNewWave(int entityID, int currentWave)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			int maxValue = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(entityID).CurrentWaveCounterComponent.maxValue;
			if (currentWave > 0 && currentWave < maxValue)
			{
				FasterReadOnlyList<AIAgentDataComponentsNode> val = entityViewsDB.QueryEntityViews<AIAgentDataComponentsNode>();
				for (int num = val.get_Count() - 1; num >= 0; num--)
				{
					AIAgentDataComponentsNode aIAgentDataComponentsNode = val.get_Item(num);
					UnregisterAIMachineCommand unregisterAIMachineCommand = commandFactory.Build<UnregisterAIMachineCommand>();
					unregisterAIMachineCommand.Initialise(aIAgentDataComponentsNode.aiBotIdData.playerId, entityViewsDB);
					unregisterAIMachineCommand.Execute();
				}
			}
		}

		private void HandleNewWaveReady(int entityID, bool ready)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (ready)
			{
				entityViewsDB.QueryEntityViews<CameraControlNode>().get_Item(0).cameraControlComponent.controlScriptEnabled = false;
				lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(SpawnAllBots)).Execute();
			}
		}

		private void SpawnAllBots(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			DictionaryEnumerator<string, PlayerDataDependency> enumerator = expectedPlayers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerDataDependency value = enumerator.get_Current().Value;
					if (!value.AiPlayer)
					{
						PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(value.PlayerName);
						preloadedMachine.rbData.set_isKinematic(true);
						if (entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>().get_Item(0).CurrentWaveCounterComponent.counterValue.get_value() > 0)
						{
							HandleHumanPlayerRespawnPointRequest();
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			_allPlayersSpawnedObservable.Dispatch();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)WaitToMove);
		}

		private IEnumerator WaitToMove()
		{
			FasterReadOnlyList<CurrentWaveTrackerEntityView> currentWaveTrackerEntityViews = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>();
			yield return null;
			AllowRobotMovements(canMove: false);
			if (currentWaveTrackerEntityViews.get_Item(0).CurrentWaveCounterComponent.counterValue.get_value() == 0)
			{
				DisableLoadingScreen();
			}
			yield return null;
			entityViewsDB.QueryEntityViews<CameraControlNode>().get_Item(0).cameraControlComponent.controlScriptEnabled = true;
			FasterReadOnlyList<WaveTransitionEntityView> waveTransitionsEntityViews = entityViewsDB.QueryEntityViews<WaveTransitionEntityView>();
			while (waveTransitionsEntityViews.get_Item(0).AnimationComponent.IsPlaying)
			{
				yield return null;
			}
			AllowRobotMovements(canMove: true);
			ITimeComponent timeTracker = entityViewsDB.QueryEntityView<CampaignWaveUpdateTimeEntityView>(207).timeComponent;
			timeTracker.timeRunning.set_value(true);
		}

		private static void DisableLoadingScreen()
		{
			MultiplayerLoadingScreen multiplayerLoadingScreen = Object.FindObjectOfType<MultiplayerLoadingScreen>();
			if (multiplayerLoadingScreen != null)
			{
				Object.Destroy(multiplayerLoadingScreen.get_transform().get_root().get_gameObject());
			}
		}

		private void AllowRobotMovements(bool canMove)
		{
			_allowMovementObserverable.Dispatch(ref canMove);
			if (canMove)
			{
				_cursorMode.Reset();
			}
			else
			{
				_cursorMode.PushNoKeyInputMode();
			}
		}
	}
}
