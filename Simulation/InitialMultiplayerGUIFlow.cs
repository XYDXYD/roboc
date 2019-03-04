using Authentication;
using Battle;
using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class InitialMultiplayerGUIFlow : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IInitialize, IInitialSimulationGUIFlow
	{
		private bool _receivedEndOfSync;

		private bool _countdownStarted;

		private GameObject _loadingScreen;

		[Inject]
		public LobbyGameStartPresenter lobbyGameStartPresenter
		{
			private get;
			set;
		}

		[Inject]
		public ICursorMode cursorMode
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
		public IContainer container
		{
			private get;
			set;
		}

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			protected get;
			set;
		}

		[Inject]
		public TeamBasePreloader teamBasePreloader
		{
			private get;
			set;
		}

		[Inject]
		public BattleLoadProgress battleLoadProgress
		{
			private get;
			set;
		}

		[Inject]
		internal AccountSanctionsSimulation accountSanctions
		{
			get;
			set;
		}

		[Inject]
		internal ChatPresenter chatPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal AISpawnerMultiplayer aiSpawner
		{
			private get;
			set;
		}

		[Inject]
		internal MultiplayerAvatars multiplayerAvatars
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkEventDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionSyncReplayer _destructionSyncReplayer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer _playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer _playerTeams
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers _battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal LocalAIsContainer _multiplayerAIs
		{
			private get;
			set;
		}

		public event Action OnGuiFlowComplete = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			cursorMode.PushFreeMode();
			_loadingScreen = Object.FindObjectOfType<MultiplayerLoadingScreen>().get_gameObject();
			CustomiseUIStyle();
		}

		protected virtual void CustomiseUIStyle()
		{
			CustomiseBattleStatsPresenterCommand customiseBattleStatsPresenterCommand = commandFactory.Build<CustomiseBattleStatsPresenterCommand>();
			customiseBattleStatsPresenterCommand.Inject(GameModeType.TeamDeathmatch);
			customiseBattleStatsPresenterCommand.Execute();
		}

		public void OnStartCountdown()
		{
			_countdownStarted = true;
		}

		public void OnReceiveEndOfSync()
		{
			_receivedEndOfSync = true;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			if (_loadingScreen != null)
			{
				DestroyLoadingScreen();
			}
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)InitialDisplay);
		}

		public virtual void CreateLobbyStartScreen()
		{
			gameObjectFactory.Build("BattleStart");
		}

		public static void LoadPlayerTeams(BattlePlayers battlePlayers, PlayerTeamsContainer playerTeams, PlayerNamesContainer playerNamesContainer)
		{
			List<PlayerDataDependency> expectedPlayersList = battlePlayers.GetExpectedPlayersList();
			string username = User.Username;
			for (int i = 0; i < expectedPlayersList.Count; i++)
			{
				PlayerDataDependency playerDataDependency = expectedPlayersList[i];
				int playerId = playerNamesContainer.GetPlayerId(playerDataDependency.PlayerName);
				if (playerDataDependency.PlayerName == username)
				{
					playerTeams.SetLocalId(playerId);
				}
				playerTeams.RegisterPlayerTeam(TargetType.Player, playerId, playerDataDependency.TeamId);
			}
		}

		public static void ClearPlayerTeams(BattlePlayers battlePlayers, PlayerTeamsContainer playerTeams, PlayerNamesContainer playerNamesContainer)
		{
			List<PlayerDataDependency> expectedPlayersList = battlePlayers.GetExpectedPlayersList();
			string username = User.Username;
			for (int i = 0; i < expectedPlayersList.Count; i++)
			{
				PlayerDataDependency playerDataDependency = expectedPlayersList[i];
				int playerId = playerNamesContainer.GetPlayerId(playerDataDependency.PlayerName);
				if (playerDataDependency.PlayerName == username)
				{
					playerTeams.RemoveLocalId();
				}
				playerTeams.UnregisterPlayerTeam(TargetType.Player, playerId);
			}
		}

		private IEnumerator PreloadMachines()
		{
			yield return _playerNamesContainer.LoadData();
			yield return _multiplayerAIs.LoadData();
			LoadPlayerTeams(_battlePlayers, _playerTeams, _playerNamesContainer);
			yield return machinePreloader.PreloadAllMachines();
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				yield return teamBasePreloader.PreloadAllAsync();
			}
			aiSpawner.SetupAIEntities();
		}

		private IEnumerator InitialDisplay()
		{
			Console.Log("initial multiplayer gui flow: InitialDisplay");
			CreateLobbyStartScreen();
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			LobbyPlayerListView loadingView = _loadingScreen.GetComponentInChildren<LobbyPlayerListView>();
			container.Inject<LobbyPlayerListView>(loadingView);
			Console.Log("Waiting for game to be able to start");
			yield return PreloadMachines();
			while (!loadingView.IsOpen())
			{
				yield return null;
			}
			cursorMode.PushNoKeyInputMode();
			commandFactory.Build<RequestLoadingProgressAllUsersCommand>().Execute();
			Console.Log("Waiting for local client to be ready");
			Console.Log("Waiting for avatar atlas..");
			yield return multiplayerAvatars.LoadAndInjectAvatars();
			Console.Log("Waiting for battle progress.IsComplete..");
			while (!battleLoadProgress.IsComplete)
			{
				yield return null;
			}
			commandFactory.Build<BroadcastLoadingProgressClientCommand>().Inject(new LoadingProgressDependency(1f)).Execute();
			yield return chatPresenter.InitializeInFlow();
			networkEventDispatcher.SendEventToServer(NetworkEvent.RequestSync, new NetworkDependency());
			NetworkEventRegistrationSimulation.RegisterSyncEvents(networkEventDispatcher, commandFactory);
			Console.Log("Waiting for sync");
			while (!_receivedEndOfSync || !_destructionSyncReplayer.isFinished)
			{
				yield return null;
			}
			NetworkEventRegistrationSimulation.RegisterIngameEvents(networkEventDispatcher, commandFactory);
			commandFactory.Build<SendLoadingCompleteCommand>().Execute();
			Console.Log("Ready to start game");
			while (!_countdownStarted)
			{
				yield return null;
			}
			DestroyLoadingScreen();
			cursorMode.PushNoKeyInputMode();
			while (!lobbyGameStartPresenter.hasBeenClosed)
			{
				yield return null;
			}
			cursorMode.Reset();
			guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			yield return accountSanctions.RefreshData();
			accountSanctions.MothershipFlowCompleted();
			this.OnGuiFlowComplete();
		}

		private void DestroyLoadingScreen()
		{
			Object.Destroy(_loadingScreen.get_transform().get_root().get_gameObject());
			_loadingScreen = null;
		}
	}
}
