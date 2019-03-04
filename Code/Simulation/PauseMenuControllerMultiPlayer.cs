using Fabric;
using RCNetwork.Events;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation
{
	internal abstract class PauseMenuControllerMultiPlayer : IGUIDisplay, IPauseMenuController, IComponent
	{
		private const float DISPLAY_TELEPORT_TIME = 2.5f;

		private readonly BattleLeftEventObservable _battleLeftEventObservable;

		protected PauseMenuWidgetSimulation _pauseMenu;

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
			protected get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
		{
			protected get;
			set;
		}

		[Inject]
		public LobbyGameStartPresenter LobbyGameStartPresenter
		{
			protected get;
			set;
		}

		[Inject]
		public BonusManager bonusManager
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

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public PauseMenuControllerMultiPlayer(BattleLeftEventObservable battleLeftEventObservable, PlayerQuitRequestCompleteObserver quitRequestCompleteObserver)
		{
			_battleLeftEventObservable = battleLeftEventObservable;
			quitRequestCompleteObserver.AddAction((Action)BackToMothership);
		}

		public void EnableBackground(bool enable)
		{
		}

		public abstract GUIShowResult Show();

		public abstract void ActivateCorrectCoolDown();

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
			ActivateCorrectCoolDown();
		}

		public void Clicked(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.QuitGame:
				QuitGame();
				break;
			case ButtonType.Confirm:
				ConfirmQuitGame();
				break;
			case ButtonType.Cancel:
				ContinueBattle();
				break;
			case ButtonType.Teleport:
				SelfDestruct();
				break;
			case ButtonType.Surrender:
				Surrender();
				break;
			case ButtonType.ControlSettings:
				ShowControlSettings();
				break;
			case ButtonType.ShowSettingsScreen:
				ShowOtherSettings();
				break;
			}
		}

		public void QuitGame()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			_pauseMenu.ShowConfirmQuitDialog(show: true);
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

		private void ConfirmQuitGame()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			eventManagerClient.SendEventToServer(NetworkEvent.PlayerQuitRequest, new NetworkDependency());
			guiInputController.ToggleCurrentScreen();
		}

		private void BackToMothership()
		{
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Leave);
			commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			_battleLeftEventObservable.Dispatch();
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

		private void Surrender()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
			commandFactory.Build<InitiateSurrenderClientCommand>().Inject(playerTeamsContainer.localPlayerId).Execute();
			guiInputController.ToggleCurrentScreen();
		}

		private IEnumerator TeleportAttemptEnd()
		{
			yield return (object)new WaitForSecondsEnumerator(2.5f);
			teleportCooldownController.TeleportAttemptEnded();
		}
	}
}
