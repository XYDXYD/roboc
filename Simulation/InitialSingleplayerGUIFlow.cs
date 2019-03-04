using Authentication;
using Battle;
using RCNetwork.Client.UNet;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class InitialSingleplayerGUIFlow : IInitialize, IInitialSimulationGUIFlow, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization
	{
		private NetworkInitialisationMockClientUnity _networkInitialisationMockClientUnity;

		[Inject]
		internal IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher dispatcher
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

		[Inject]
		internal MachinePreloader machinePreloader
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
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
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
		internal PlayerNamesContainer _playerNamesContainer
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
		internal RobotSanctionController robotSanctionController
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching worldSwitch
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

		public event Action OnGuiFlowComplete = delegate
		{
		};

		public InitialSingleplayerGUIFlow(NetworkInitialisationMockClientUnity networkInitialisationMockClientUnity)
		{
			_networkInitialisationMockClientUnity = networkInitialisationMockClientUnity;
		}

		public void OnStartCountdown()
		{
		}

		public void OnReceiveEndOfSync()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			dispatcher.OnPlayerSpawnedIn += HandleonSpawnedIn;
			StartPlanetSinglePlayerClientCommand startPlanetSinglePlayerClientCommand = commandFactory.Build<StartPlanetSinglePlayerClientCommand>();
			startPlanetSinglePlayerClientCommand.Inject(worldSwitch);
			startPlanetSinglePlayerClientCommand.Execute();
			CustomiseUIStyle();
		}

		protected virtual void CustomiseUIStyle()
		{
			CustomiseBattleStatsPresenterCommand customiseBattleStatsPresenterCommand = commandFactory.Build<CustomiseBattleStatsPresenterCommand>();
			customiseBattleStatsPresenterCommand.Inject(GameModeType.PraticeMode);
			customiseBattleStatsPresenterCommand.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			this.OnGuiFlowComplete = delegate
			{
			};
		}

		public virtual void OnFrameworkInitialized()
		{
			_networkInitialisationMockClientUnity.Initialize();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)InitialDisplayBase);
		}

		private void HandleonSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, spawnInParameters.playerId))
			{
				guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			}
		}

		private IEnumerator PreloadMachines()
		{
			commandFactory.Build<GeneratePlayerIDsMockClientCommand>().Execute();
			InitialMultiplayerGUIFlow.LoadPlayerTeams(_battlePlayers, playerTeamsContainer, _playerNamesContainer);
			List<PlayerDataDependency> expectedPlayersList = _battlePlayers.GetExpectedPlayersList();
			string username = User.Username;
			List<int> list = new List<int>();
			foreach (PlayerDataDependency item in expectedPlayersList)
			{
				if (item.AiPlayer)
				{
					list.Add(_playerNamesContainer.GetPlayerId(item.PlayerName));
				}
			}
			PlayerIDsDependency dependency = new PlayerIDsDependency(list.ToArray());
			commandFactory.Build<SetHostedAIsClientCommand>().Inject(dependency).Execute();
			yield return machinePreloader.PreloadAllMachines();
		}

		private IEnumerator InitialDisplayBase()
		{
			yield return PreloadMachines();
			yield return InitialDisplay();
			int localId = _playerNamesContainer.GetPlayerId(User.Username);
			_networkInitialisationMockClientUnity.Start(localId);
			ValidateRobot();
			yield return chatPresenter.InitializeInFlow();
			yield return accountSanctions.RefreshData();
			DisableLoadingScreen();
			commandFactory.Build<StartGameClientCommand>().Execute();
			cursorMode.PopFreeMode();
			cursorMode.PopFreeMode();
			this.OnGuiFlowComplete();
		}

		private void DisableLoadingScreen()
		{
			MultiplayerLoadingScreen multiplayerLoadingScreen = Object.FindObjectOfType<MultiplayerLoadingScreen>();
			if (multiplayerLoadingScreen != null)
			{
				Object.Destroy(multiplayerLoadingScreen.get_transform().get_root().get_gameObject());
			}
		}

		protected virtual IEnumerator InitialDisplay()
		{
			yield return multiplayerAvatars.LoadAndInjectAvatars();
		}

		protected void ValidateRobot()
		{
			IValidateCurrentMachineRequest validateCurrentMachineRequest = serviceRequestFactory.Create<IValidateCurrentMachineRequest>();
			validateCurrentMachineRequest.Inject(LobbyType.Solo);
			validateCurrentMachineRequest.SetAnswer(new ServiceAnswer<ValidateCurrentMachineResult>(OnRobotValidated, OnValidateFailed));
			validateCurrentMachineRequest.Execute();
		}

		private void OnRobotValidated(ValidateCurrentMachineResult currentMachineResult)
		{
			switch (currentMachineResult)
			{
			case ValidateCurrentMachineResult.MachineNotValid:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strInvalidRobot"), StringTableBase<StringTable>.Instance.GetString("strInvalidRobotPlayError"), StringTableBase<StringTable>.Instance.GetString("strOK"), ReturnToMothership));
				break;
			case ValidateCurrentMachineResult.MachineHasSanction:
				Console.Log("EnterMatchMakingFlow() - false returned, leaving queue, show error.");
				TaskRunner.get_Instance().Run(robotSanctionController.CheckRobotSanction(string.Empty, delegate
				{
				}, ReturnToMothership));
				break;
			}
		}

		private void OnValidateFailed(ServiceBehaviour serviceBehaviour)
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strInvalidRobot"), StringTableBase<StringTable>.Instance.GetString("strInvalidRobotPlayError"), StringTableBase<StringTable>.Instance.GetString("strOK"), ReturnToMothership));
		}

		private void ReturnToMothership()
		{
			SwitchToMothershipCommand switchToMothershipCommand = commandFactory.Build<SwitchToMothershipCommand>();
			switchToMothershipCommand.Execute();
		}
	}
}
