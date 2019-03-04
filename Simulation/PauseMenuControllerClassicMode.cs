using Fabric;
using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation
{
	internal sealed class PauseMenuControllerClassicMode : IGUIDisplay, IPauseMenuController, IInitialize, IWaitForFrameworkDestruction, IComponent
	{
		private const float DISPLAY_TELEPORT_TIME = 2.5f;

		private BattleLeftEventObservable _observable;

		private PauseMenuWidgetSimulation _pauseMenu;

		[Inject]
		public ICommandFactory commandFactory
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
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerClient eventManagerClient
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

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public event Action OnShowPauseMenu = delegate
		{
		};

		public PauseMenuControllerClassicMode(BattleLeftEventObservable observable)
		{
			_observable = observable;
		}

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(HandleOnGameStarted);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(HandleOnGameStarted);
		}

		private void HandleOnGameStarted()
		{
			UIButton[] selfDestructClassicButtons = _pauseMenu.selfDestructClassicButtons;
			for (int i = 0; i < selfDestructClassicButtons.Length; i++)
			{
				selfDestructClassicButtons[i].set_isEnabled(true);
			}
		}

		public void EnableBackground(bool enable)
		{
		}

		public GUIShowResult Show()
		{
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerTeamsContainer.localPlayerId))
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.BrawlEliminationMode);
			}
			else
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.Spectator);
			}
			this.OnShowPauseMenu();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_pauseMenu.Hide();
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
				ShowSelfDestructConfirmation();
				break;
			case ButtonType.SelfDestructConfirm:
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

		private void ShowControlSettings()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ShowScreen(GuiScreens.ControlsScreen);
		}

		private void ShowOtherSettings()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ShowScreen(GuiScreens.SettingsScreen);
		}

		private void ContinueBattle()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ToggleCurrentScreen();
		}

		private void ShowSelfDestructConfirmation()
		{
			_pauseMenu.ShowClassicConfirmSelfDestructDialog(show: true);
		}

		private void SelfDestruct()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			teleportCooldownController.TeleportAttemptStarted();
			if (teleportCooldownController.TeleportIsAllowed())
			{
				_observable.Dispatch();
				TeleportOutSelfDestructCommand teleportOutSelfDestructCommand = commandFactory.Build<TeleportOutSelfDestructCommand>();
				teleportOutSelfDestructCommand.Execute();
			}
			guiInputController.ToggleCurrentScreen();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TeleportAttemptEnd);
		}

		private IEnumerator TeleportAttemptEnd()
		{
			yield return (object)new WaitForSecondsEnumerator(2.5f);
			teleportCooldownController.TeleportAttemptEnded();
		}

		private void BackToMothership()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Leave);
			commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			guiInputController.ToggleCurrentScreen();
			eventManagerClient.SendEventToServer(NetworkEvent.ClientDisconnecting, new PlayerIdDependency(playerTeamsContainer.localPlayerId));
		}
	}
}
