using Authentication;
using Battle;
using PlayMaker;
using PlayMaker.Tutorial;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class SimulationTutorialController : TutorialControllerBase, IGUIDisplay, IInitialize, IWaitForFrameworkDestruction, IComponent
	{
		private SimulationTutorialScreen _tutorialDisplay;

		private bool _targetRobotDestroyed;

		private const int LOCAL_PLAYER_ID = 0;

		[Inject]
		internal BattlePlayers battlePlayers
		{
			get;
			set;
		}

		[Inject]
		internal ISpawnPointManager spawnPointManager
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
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.SimulationTutorialScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => false;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => true;

		public bool isReady => _tutorialDisplay != null;

		public HudStyle battleHudStyle => HudStyle.Full;

		public SimulationTutorialController(TutorialTipObservable observeable)
			: base(observeable)
		{
		}

		public void EnableBackground(bool enable)
		{
		}

		protected override void RegisterContextSpecificRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler)
		{
			RegisterPlayMakerRequestHandler(typeof(CheckTargetRobotDestroyedRequest), HandleCheckTargetRobotDestroyedRequest);
		}

		protected override void RegisterContextSpecificCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegisterPlayMakerCommandHandlerAction)
		{
			RegisterPlayMakerCommandHandlerAction(HandleLaunchIntoTestModeCommandExecution, typeof(LaunchIntoTestModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSwitchIntoBuildModeCommandExecution, typeof(SwitchIntoBuildModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleChangeToolModeCommandExecution, typeof(ChangeToolModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleTutorialFinishedCommandExecution, typeof(FinishedReachedTutorialNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSpawnEnemyIntoTestModeCommandExecution, typeof(SpawnEnemyIntoTestModeNodeParameters));
			RegisterPlayMakerCommandHandlerAction(HandleShowLoadingScreenCommandExecution, typeof(ShowFullscreenLoadingScreenNodeCommandParameters));
			RegisterPlayMakerCommandHandlerAction(HandleHideLoadingScreenCommandExecution, typeof(HideFullscreenLoadingScreenNodeCommandParameters));
		}

		private void HandleSpawnEnemyIntoTestModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			List<PlayerDataDependency> expectedPlayersList = battlePlayers.GetExpectedPlayersList();
			for (int i = 0; i < expectedPlayersList.Count; i++)
			{
				PlayerDataDependency playerDataDependency = expectedPlayersList[i];
				if (playerDataDependency.PlayerName != User.Username)
				{
					int playerId = playerNamesContainer.GetPlayerId(playerDataDependency.PlayerName);
					SpawningPoint nextFreeSpawnPoint = spawnPointManager.GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType.TestModeEnemy, playerId);
					PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(playerDataDependency.PlayerName);
					Vector3 worldCenterOfMass = preloadedMachine.rbData.get_worldCenterOfMass();
					MachineDefinitionDependency machineDefinitionDependency = new MachineDefinitionDependency(100, playerDataDependency.TeamId, nextFreeSpawnPoint.get_transform().get_position(), nextFreeSpawnPoint.get_transform().get_rotation(), 0, worldCenterOfMass);
					RegisterTutorialAIMachineCommand registerTutorialAIMachineCommand = base.commandFactory.Build<RegisterTutorialAIMachineCommand>();
					registerTutorialAIMachineCommand.Initialise(machineDefinitionDependency.owner, machineDefinitionDependency.teamId, playerDataDependency.PlayerName, playerDataDependency.DisplayName, preloadedMachine, nextFreeSpawnPoint, "Explosion");
					registerTutorialAIMachineCommand.Execute();
				}
			}
		}

		public override void ActivateLoadingGUI()
		{
			Console.Log("TODO: display loading screen in Simulation when playmaker executes requests");
		}

		public override void DeactivateLoadingGUI()
		{
			Console.Log("TODO: hide loading screen in Simulation when playmaker executes requests");
		}

		public void HandleLaunchIntoTestModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			Console.Log("Launch Into Test Mode Command disregarded, because you are already in test mode.");
		}

		private void HandleShowLoadingScreenCommandExecution(IPlaymakerCommandInputParameters obj)
		{
			Console.Log("Show Full Screen Loading Screen Command disregarded, because you are already in test mode.");
		}

		private void HandleHideLoadingScreenCommandExecution(IPlaymakerCommandInputParameters obj)
		{
			Console.Log("Hide Full Screen Loading Screen Command disregarded, because you are already in test mode.");
		}

		public void HandleSwitchIntoBuildModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			Console.Log("Playmaker warning: trying to invoke a command that is not available in this context (Switch to build mode) - you are in the test mode context, so Switch into build mode command will do nothing.");
		}

		public void HandleChangeToolModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			Console.Log("Playmaker warning: trying to invoke a command that is not available in this context (change tool mode) - you are in the test mode context, so Change tool mode command will do nothing.");
		}

		public void HandleTutorialFinishedCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			base.commandFactory.Build<FinishedTutorialCommand>().Execute();
		}

		private void HandleCheckTargetRobotDestroyedRequest(IPlayMakerDataRequest request)
		{
			bool targetRobotDestroyed = _targetRobotDestroyed;
			request.AssignResults(new CheckTargetRobotDestroyedRequestReturn(targetRobotDestroyed));
			request.Execute();
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += HandleOnMachineKilled;
		}

		private void HandleOnMachineKilled(int ownerId, int shooterId)
		{
			if (shooterId == 0)
			{
				_targetRobotDestroyed = true;
			}
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_tutorialDisplay = null;
		}

		public override void SetDisplay(TutorialScreenBase display)
		{
			_tutorialDisplay = (display as SimulationTutorialScreen);
		}

		public override bool IsActive()
		{
			return _tutorialDisplay.IsActive();
		}

		public override void ShowTutorialScreenAndActivateFSM()
		{
			base.guiInputController.ShowScreen(GuiScreens.SimulationTutorialScreen);
		}

		public override void HideTutorialScreen()
		{
			base.guiInputController.ForceCloseJustThisScreen(GuiScreens.SimulationTutorialScreen);
			_tutorialDisplay.HideScreen();
		}

		public GUIShowResult Show()
		{
			_tutorialDisplay.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_tutorialDisplay.HideScreen();
			return true;
		}
	}
}
