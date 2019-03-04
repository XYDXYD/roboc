using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation
{
	internal sealed class PauseMenuControllerPitMode : IGUIDisplay, IPauseMenuController, IComponent
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
		public PlayerTeamsContainer playerTeamsContainer
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
		public LobbyGameStartPresenter LobbyGameStartPresenter
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
		public GameStateClient gameStateClient
		{
			private get;
			set;
		}

		public bool doesntHideOnSwitch => false;

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

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public event Action OnShowPauseMenu = delegate
		{
		};

		public PauseMenuControllerPitMode(BattleLeftEventObservable observable)
		{
			_observable = observable;
		}

		public void EnableBackground(bool enable)
		{
		}

		public GUIShowResult Show()
		{
			if (LobbyGameStartPresenter.hasBeenClosed && livePlayersContainer.IsPlayerAlive(TargetType.Player, playerTeamsContainer.localPlayerId))
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.PitMode);
				this.OnShowPauseMenu();
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowed;
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

		private void ContinueBattle()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			guiInputController.ToggleCurrentScreen();
		}

		private void BackToMothership()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Leave);
			commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			guiInputController.ToggleCurrentScreen();
			_observable.Dispatch();
			bonusManager.IgnoreReplyFromGameServer();
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
