using Fabric;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation
{
	internal sealed class PauseMenuControllerTestMode : IGUIDisplay, IPauseMenuController, IComponent
	{
		private const float DISPLAY_TELEPORT_TIME = 2.5f;

		private PauseMenuWidgetSimulation _pauseMenu;

		[Inject]
		public IPauseManager pauseManager
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
		public IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager networkMachineManager
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public ITeleportCooldownController teleportCooldownController
		{
			private get;
			set;
		}

		[Inject]
		public IInitialSimulationGUIFlow simulationGUIFlow
		{
			private get;
			set;
		}

		[Inject]
		internal ITutorialController tutorialController
		{
			private get;
			set;
		}

		public PauseMenuWidgetSimulation pauseMenu
		{
			set
			{
				_pauseMenu = value;
			}
		}

		public GuiScreens screenType => GuiScreens.PauseMenu;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public event Action OnShowPauseMenu = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		public GUIShowResult Show()
		{
			if (tutorialController.TutorialInProgress())
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.TestModeTutorial);
			}
			else
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.TestMode);
			}
			this.OnShowPauseMenu();
			pauseManager.Pause(pause: true);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_pauseMenu.Hide();
			pauseManager.Pause(pause: false);
			return true;
		}

		public bool IsActive()
		{
			if (_pauseMenu == null)
			{
				return false;
			}
			return _pauseMenu.IsActive();
		}

		public void SetWidget(IPauseMenuWidget widget)
		{
			pauseMenu = (widget as PauseMenuWidgetSimulation);
		}

		public void Clicked(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.Confirm:
				BackToMothership();
				break;
			case ButtonType.SkipTutorial:
				SkipTutorial();
				break;
			case ButtonType.Cancel:
				ContinueClicked();
				break;
			case ButtonType.Teleport:
				SelfDestruct();
				break;
			case ButtonType.ControlSettings:
				ShowControlSettings();
				break;
			case ButtonType.ShowSettingsScreen:
				ShowOtherSettings();
				break;
			}
		}

		private void BackToMothership()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			guiInputController.ToggleCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}

		private void SkipTutorial()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			commandFactory.Build<SkipTutorialFromSimulationCommand>().Execute();
			guiInputController.ToggleCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}

		private void ContinueClicked()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ToggleCurrentScreen();
		}

		private void ShowOtherSettings()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ShowScreen(GuiScreens.SettingsScreen);
		}

		private void ShowControlSettings()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ShowScreen(GuiScreens.ControlsScreen);
		}

		private void SelfDestruct()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			teleportCooldownController.TeleportAttemptStarted();
			if (teleportCooldownController.TeleportIsAllowed())
			{
				OnSelfDestructClientCommand onSelfDestructClientCommand = commandFactory.Build<OnSelfDestructClientCommand>();
				onSelfDestructClientCommand.Execute();
			}
			guiInputController.ToggleCurrentScreen();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TeleportAttemptEnd);
		}

		private IEnumerator TeleportAttemptEnd()
		{
			yield return (object)new WaitForSecondsEnumerator(2.5f);
			teleportCooldownController.TeleportAttemptEnded();
		}
	}
}
