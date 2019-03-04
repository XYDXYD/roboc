using Battle;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Simulation
{
	internal class InitialSingleplayerGUIFlowCampaign : IInitialize, IInitialSimulationGUIFlow, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization
	{
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
			TaskRunner.get_Instance().Run((Func<IEnumerator>)InitialDisplayBase);
		}

		private void HandleonSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, spawnInParameters.playerId))
			{
				guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			}
		}

		private IEnumerator InitialDisplayBase()
		{
			ValidateRobot();
			yield return accountSanctions.RefreshData();
			cursorMode.PopFreeMode();
			cursorMode.PopFreeMode();
			this.OnGuiFlowComplete();
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
