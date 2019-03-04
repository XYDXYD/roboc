using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal sealed class PauseMenuControllerPracticeMode : IGUIDisplay, IPauseMenuController, IInitialize, IWaitForFrameworkDestruction, IComponent
	{
		private PauseMenuWidgetSimulation _pauseMenu;

		private bool _gameStarted;

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
		public BonusManager bonusManager
		{
			private get;
			set;
		}

		[Inject]
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public GameStateClient gameStateClient
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

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public event Action OnShowPauseMenu = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		public void OnDependenciesInjected()
		{
			gameStartDispatcher.Register(HandleOnGameStart);
		}

		public void OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(HandleOnGameStart);
		}

		public GUIShowResult Show()
		{
			if (!_gameStarted)
			{
				return GUIShowResult.NotShowed;
			}
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerTeamsContainer.localPlayerId))
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.PracticeMode);
			}
			else
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.Spectator);
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
			case ButtonType.Cancel:
				ContinueBattle();
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
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Leave);
			commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			guiInputController.ToggleCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			bonusManager.IgnoreReplyFromGameServer();
		}

		private void ContinueBattle()
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
			OnSelfDestructClientCommand onSelfDestructClientCommand = commandFactory.Build<OnSelfDestructClientCommand>();
			onSelfDestructClientCommand.Execute();
			if (guiInputController.GetActiveScreen() == screenType)
			{
				guiInputController.ToggleCurrentScreen();
			}
		}

		private void HandleOnGameStart()
		{
			_gameStarted = true;
		}
	}
}
