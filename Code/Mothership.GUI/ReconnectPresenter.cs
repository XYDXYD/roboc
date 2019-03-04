using LobbyServiceLayer;
using LobbyServiceLayer.Photon;
using Services.Web.Photon;
using Simulation;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using Utility;

namespace Mothership.GUI
{
	internal class ReconnectPresenter : IGUIDisplay, IComponent
	{
		private enum Choice
		{
			None,
			Reconnect,
			Dismiss
		}

		private Choice _choice;

		private ReconnectScreenView _view;

		[Inject]
		internal ILobbyRequestFactory _lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory _servicesRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory _commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController _guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter _loadingIconPresenter
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.ReconnectScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public void CheckReconnection()
		{
			Console.Log("CheckReconnection");
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ReconnectMothershipFlow);
		}

		public void SetView(ReconnectScreenView view)
		{
			_view = view;
		}

		private IEnumerator ReconnectMothershipFlow()
		{
			_loadingIconPresenter.NotifyLoading("GetReconnectableGame");
			IPingReconnectableGameRequest pingRequest = _servicesRequestFactory.Create<IPingReconnectableGameRequest>();
			TaskService<bool> pingTask = new TaskService<bool>(pingRequest);
			yield return new HandleTaskServiceWithError(pingTask, delegate
			{
				_loadingIconPresenter.NotifyLoading("GetReconnectableGame");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("GetReconnectableGame");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("GetReconnectableGame");
			if (!pingTask.succeeded)
			{
				Console.LogError("Couldn't check reconnectable game from services");
				yield break;
			}
			if (!pingTask.result)
			{
				Console.Log("No game to reconnect (ping)");
				yield break;
			}
			_loadingIconPresenter.NotifyLoading("GetReconnectableGame");
			IGetReconnectableGameRequest getReconnectableGameRequest = _lobbyRequestFactory.Create<IGetReconnectableGameRequest>();
			TaskService<EnterBattleDependency> task = new TaskService<EnterBattleDependency>(getReconnectableGameRequest);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIconPresenter.NotifyLoading("GetReconnectableGame");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("GetReconnectableGame");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("GetReconnectableGame");
			if (!task.succeeded)
			{
				Console.LogError("Couldn't check reconnectable game from lobby, user won't be able to reconnect");
				yield break;
			}
			if (task.result == null)
			{
				Console.Log("No game to reconnect (lobby)");
				yield break;
			}
			EnterBattleDependency enterBattleDependency = task.result;
			Console.Log("Found a game to reconnect, asking the player");
			_guiInputController.ShowScreen(screenType);
			TaskRunner.get_Instance().Run(PollGameAvailability());
			_choice = Choice.None;
			while (_choice == Choice.None)
			{
				yield return null;
			}
			_guiInputController.CloseCurrentScreen();
			if (_choice == Choice.Reconnect)
			{
				Console.Log("Reconnecting");
				_commandFactory.Build<ReconnectToGameCommand>().Inject(enterBattleDependency).Execute();
			}
			else
			{
				Console.Log("Reconnect cancelled, choice=" + _choice);
				yield return UnregisterFromGame();
				PhotonLobbyUtility.Instance.Disconnect();
			}
		}

		private IEnumerator UnregisterFromGame()
		{
			_loadingIconPresenter.NotifyLoading("UnregisterFromReconnectableGame");
			IUnregisterPlayerFromReconnectableGameClientRequest request = _lobbyRequestFactory.Create<IUnregisterPlayerFromReconnectableGameClientRequest>();
			TaskService task = new TaskService(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIconPresenter.NotifyLoading("UnregisterFromReconnectableGame");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("UnregisterFromReconnectableGame");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("UnregisterFromReconnectableGame");
		}

		private IEnumerator PollGameAvailability()
		{
			TaskService<bool> task;
			do
			{
				yield return (object)new WaitForSecondsEnumerator(5f);
				IPingReconnectableGameRequest request = _servicesRequestFactory.Create<IPingReconnectableGameRequest>();
				task = new TaskService<bool>(request);
				yield return task;
				if (_choice != 0)
				{
					yield break;
				}
				if (!task.succeeded)
				{
					Console.LogError("Can't ping reconnectable game, assume it ended");
					_view.GameEnded();
					yield break;
				}
			}
			while (task.result);
			Console.Log("Ping returned no reconnectable game");
			_view.GameEnded();
		}

		internal void OnAskReconnect()
		{
			_choice = Choice.Reconnect;
		}

		internal void OnAskCancel()
		{
			_choice = Choice.Dismiss;
		}

		public GUIShowResult Show()
		{
			_view.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_view.Hide();
			return true;
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return !(_view == null) && _view.get_gameObject().get_activeSelf();
		}
	}
}
