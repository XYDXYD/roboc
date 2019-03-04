using Svelto.Command;
using Svelto.ES.Legacy;

namespace Mothership
{
	internal sealed class WorldSwitchInputMothership : IHandleWorldSwitchInputPracticeModeShortcut, IHandleWorldSwitchInputTestModeShortcut, IHandleWorldSwitchInputQuickPlayModeShortcut, IHandleWorldSwitchInputBuildModeShortcut, IHandleWorldSwitchInputMainMenuModeShortcut, IHandleWorldSwitchInputBrawlShortcut, IInputComponent, IComponent
	{
		private IGUIInputControllerMothership _guiInputController;

		private ICommandFactory _commandFactory;

		private DesiredGameMode _desiredGameMode;

		public WorldSwitchInputMothership(ICommandFactory commandFactory, IGUIInputControllerMothership guiInputController, NormalBattleAvailability normalBattleAvailability, TeamDeathMatchAvailability teamDeathMatchAvailabilty, DesiredGameMode desiredGameMode)
		{
			_commandFactory = commandFactory;
			_guiInputController = guiInputController;
			_desiredGameMode = desiredGameMode;
		}

		private bool AreShortcutsAllowed()
		{
			return _guiInputController.GetShortCutMode() == ShortCutMode.AllShortCuts || _guiInputController.GetShortCutMode() == ShortCutMode.OnlyGUINoSwitching || _guiInputController.GetShortCutMode() == ShortCutMode.BuildShortCuts;
		}

		public void HandleWorldSwitchInputTestModeShortcut()
		{
			if (CanSwitchToTestMode() && AreShortcutsAllowed())
			{
				_guiInputController.ToggleScreen(GuiScreens.TestMode);
			}
		}

		private bool CanSwitchToTestMode()
		{
			return _guiInputController.GetActiveScreen() == GuiScreens.BuildMode || _guiInputController.GetActiveScreen() == GuiScreens.Garage;
		}

		public void HandleWorldSwitchInputPracticeModeShortcut()
		{
			if (IsEnterPlanetDialogueActive() && AreShortcutsAllowed())
			{
				_commandFactory.Build<KickStartSinglePlayerCommand>().Execute();
			}
		}

		private bool IsEnterPlanetDialogueActive()
		{
			return _guiInputController.GetActiveScreen() == GuiScreens.PlayScreen;
		}

		void IHandleWorldSwitchInputQuickPlayModeShortcut.HandleWorldSwitchInputQuickPlayModeShortcut()
		{
			EnterPlanetDialogueController enterPlanetDialogueController = _guiInputController.GetControllerForScreen(GuiScreens.PlayScreen) as EnterPlanetDialogueController;
			if (AreShortcutsAllowed() && IsEnterPlanetDialogueActive())
			{
				_desiredGameMode.DesiredMode = LobbyType.QuickPlay;
				_guiInputController.ToggleScreen(GuiScreens.BattleCountdown);
			}
		}

		void IHandleWorldSwitchInputBuildModeShortcut.HandleWorldSwitchInputBuildModeShortcut()
		{
			EnterPlanetDialogueController enterPlanetDialogueController = _guiInputController.GetControllerForScreen(GuiScreens.PlayScreen) as EnterPlanetDialogueController;
			if (enterPlanetDialogueController.IsModeAvailable(GameModeType.EditMode) && _guiInputController.GetActiveScreen() == GuiScreens.Garage && _guiInputController.GetShortCutMode() == ShortCutMode.AllShortCuts)
			{
				SwitchWorldDependency dependency = new SwitchWorldDependency("RC_BuildMode", _fastSwitch: true);
				_commandFactory.Build<SwitchToBuildModeCommand>().Inject(dependency).Execute();
			}
		}

		void IHandleWorldSwitchInputMainMenuModeShortcut.HandleWorldSwitchInputMainMenuModeShortcut()
		{
			if (WorldSwitching.IsInBuildMode() && _guiInputController.GetShortCutMode() == ShortCutMode.BuildShortCuts)
			{
				SwitchWorldDependency dependency = new SwitchWorldDependency("RC_Mothership", _fastSwitch: true);
				_commandFactory.Build<SwitchToGarageCommand>().Inject(dependency).Execute();
			}
		}

		void IHandleWorldSwitchInputBrawlShortcut.HandleWorldSwitchInputBrawlShortcut()
		{
			if (IsEnterPlanetDialogueActive() && AreShortcutsAllowed())
			{
				_commandFactory.Build<SwitchToBrawlCommand>().Execute();
			}
		}
	}
}
