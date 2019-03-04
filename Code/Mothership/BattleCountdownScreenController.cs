using Services.Web.Photon;
using Simulation;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class BattleCountdownScreenController : IGUIDisplay, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization, IComponent
	{
		public enum BattleCountdownState
		{
			Idle,
			ConnectingToLobby,
			ConnectedToLobby,
			WaitingForRobotValidation,
			WaitingForRejoinAction,
			ShowingAFKWarning,
			BattleFound,
			WaitToExit,
			EnteringGame,
			Exited
		}

		private const float TIME_OUT = 15f;

		private BattleCountdownState _state;

		private float _waitForExitStartTime;

		private bool _registerTimeoutErrorWindowPending;

		private ITaskRoutine _exitTimeOutCheckTask;

		private GameObject _loadingIcon;

		private bool _deferredErrorToShow;

		private string _deferredErrorTitleKey;

		private string _deferredErrorBody;

		private string _deferredErrorReason;

		private Action _deferredErrorRetry;

		private BattleCountdownScreen _view;

		private AFKBlockTimerScreen _afkBlockTimerView;

		[Inject]
		public LobbyCountdown lobbyCountdown
		{
			private get;
			set;
		}

		[Inject]
		public AFKBlockTimer afkBlockTimer
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public GenericInfoDisplay infoDisplay
		{
			private get;
			set;
		}

		[Inject]
		public EnterBattleChecker enterBattleChecker
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal DesiredGameMode desiredGameMode
		{
			private get;
			set;
		}

		[Inject]
		internal LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingPresenter
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
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.BattleCountdown;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public event Action OnBattleCountdownShown = delegate
		{
		};

		public event Action OnBattleCountdownHidden = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			lobbyPresenter.EntryBlockedEvent += HandleOnBlockFromEnteringLobby;
			_exitTimeOutCheckTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)CheckExitTimeout);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			lobbyPresenter.EntryBlockedEvent -= HandleOnBlockFromEnteringLobby;
		}

		public void SetBattleCountdownScreenView(BattleCountdownScreen view)
		{
			_view = view;
		}

		public void SetAFKBlockTimerScreenView(AFKBlockTimerScreen view)
		{
			_afkBlockTimerView = view;
		}

		public bool IsActive()
		{
			return _state != BattleCountdownState.Idle;
		}

		public GUIShowResult Show()
		{
			_view.ShowInQueueAnimation();
			_deferredErrorToShow = false;
			if (enterBattleChecker.IsMachineValidForBattle())
			{
				_state = BattleCountdownState.WaitingForRobotValidation;
				lobbyCountdown.onSecondsChanged += UpdateLobbyLabels;
				afkBlockTimer.onSecondsChanged += SetBlockedLobbyCountDown;
				lobbyCountdown.StartCountUp();
				_view.Show();
				_afkBlockTimerView.Hide();
				this.OnBattleCountdownShown();
				CheckParty();
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowedSlim;
		}

		private void CheckParty()
		{
			ITaskRoutine val = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			val.SetEnumerator(CheckPartyTiers()).Start((Action<PausableTaskException>)null, (Action)CheckPartyStopped);
		}

		private void CheckPartyStopped()
		{
			loadingPresenter.NotifyLoadingDone("PartyTierCheck");
		}

		private IEnumerator CheckPartyTiers()
		{
			loadingPresenter.NotifyLoading("PartyTierCheck");
			ICheckPartyRobotTiersRequest checkPartyTierRequest = socialRequestFactory.Create<ICheckPartyRobotTiersRequest>();
			TaskService<bool> task = new TaskService<bool>(checkPartyTierRequest);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("PartyTierCheck");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("PartyTierCheck");
			}).GetEnumerator();
			loadingPresenter.NotifyLoadingDone("PartyTierCheck");
			if (task.succeeded)
			{
				Console.Log("Check party tiers - result = " + task.result);
				if (task.result)
				{
					ValidateRobot();
					yield break;
				}
				ChangeStateToExited();
				guiInputController.CloseCurrentScreen();
				string headerText = Localization.Get("strCannotPlayRobotTierPartyTitle", true);
				string bodyText = Localization.Get("strCannotPlayRobotTierPartyBody", true);
				ErrorWindow.ShowErrorWindow(new GenericErrorData(headerText, bodyText));
			}
			else
			{
				Console.Log("Check party tiers - the task failed");
				yield return null;
			}
		}

		private void ValidateRobot()
		{
			IValidateCurrentMachineRequest validateCurrentMachineRequest = serviceRequestFactory.Create<IValidateCurrentMachineRequest>();
			validateCurrentMachineRequest.Inject(desiredGameMode.DesiredMode);
			validateCurrentMachineRequest.SetAnswer(new ServiceAnswer<ValidateCurrentMachineResult>(OnValidateReturn, OnValidateRequestFail));
			validateCurrentMachineRequest.Execute();
		}

		private void OnValidateReturn(ValidateCurrentMachineResult validRobot)
		{
			switch (validRobot)
			{
			case ValidateCurrentMachineResult.MachineIsValid:
				HandleRobotIsValid();
				break;
			case ValidateCurrentMachineResult.MachineNotValid:
				HandleRobotIsNotValid();
				break;
			case ValidateCurrentMachineResult.MachineHasNoWeapons:
			{
				Console.Log("EnterMatchMakingFlow() - false returned, leaving queue, show error.");
				ChangeStateToExited();
				guiInputController.CloseCurrentScreen();
				string headerText2 = Localization.Get("strRobotUnsuitableForPlayNoWeaponsTitle", true);
				string bodyText2 = Localization.Get("strRobotUnsuitableForPlayNoWeaponsBody", true);
				ErrorWindow.ShowErrorWindow(new GenericErrorData(headerText2, bodyText2));
				break;
			}
			case ValidateCurrentMachineResult.MachineHasNoMovementParts:
			{
				Console.Log("EnterMatchMakingFlow() - false returned, leaving queue, show error.");
				ChangeStateToExited();
				guiInputController.CloseCurrentScreen();
				string headerText = Localization.Get("strRobotUnsuitableForPlayNoMovementPartsTitle", true);
				string bodyText = Localization.Get("strRobotUnsuitableForPlayNoMovementPartsBody", true);
				ErrorWindow.ShowErrorWindow(new GenericErrorData(headerText, bodyText));
				break;
			}
			case ValidateCurrentMachineResult.MachineHasSanction:
				HandleRobotHasSanction();
				break;
			}
		}

		private void OnValidateRequestFail(ServiceBehaviour serviceBehaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			HandleRobotIsNotValid();
		}

		private void HandleRobotHasSanction()
		{
			Console.Log("EnterMatchMakingFlow() - false returned, leaving queue, show error.");
			TaskRunner.get_Instance().Run(robotSanctionController.CheckRobotSanction(garagePresenter.CurrentRobotIdentifier.ToString(), delegate(RobotSanctionData sanction)
			{
				TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
			}, delegate
			{
				ChangeStateToExited();
				guiInputController.CloseCurrentScreen();
			}));
		}

		private IEnumerator OnRobotSanctionTaskSucceeded(RobotSanctionData robotSanction)
		{
			if (robotSanction != null)
			{
				yield return garagePresenter.RefreshGarageData();
				garagePresenter.ShowGarageSlots();
				garagePresenter.LoadAndBuildRobot();
				garagePresenter.SelectCurrentGarageSlot();
			}
		}

		private void HandleRobotIsNotValid()
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strGenericError"), StringTableBase<StringTable>.Instance.GetString("strRobotValidError")));
			guiInputController.CloseCurrentScreen();
		}

		private void HandleRobotIsValid()
		{
			if (_state == BattleCountdownState.WaitingForRobotValidation)
			{
				_view.SetWaitingForBattleModeHeader(desiredGameMode.DesiredMode);
				ChangeStateToRegistrationRequested();
			}
		}

		public void SetBattleCountDownState(BattleCountdownState newState)
		{
			_state = newState;
		}

		internal void ChangeStateToJoinedLobby()
		{
			SetBattleCountDownState(BattleCountdownState.ConnectedToLobby);
		}

		private void ChangeStateToReregistrationRequested()
		{
			SetBattleCountDownState(BattleCountdownState.ConnectingToLobby);
			lobbyPresenter.TryEnterMatchmakingQueue();
		}

		public void ChangeStateToRegistrationRequested()
		{
			SetBattleCountDownState(BattleCountdownState.ConnectingToLobby);
			lobbyPresenter.TryEnterMatchmakingQueue();
			lobbyPresenter.LeftQueueEvent += HandleOnDeregisteredFromGame;
		}

		internal void ChangeStateToExited()
		{
			_state = BattleCountdownState.Exited;
		}

		public bool DoesStateAllowEnterGame()
		{
			bool flag = false;
			switch (_state)
			{
			case BattleCountdownState.Idle:
			case BattleCountdownState.WaitingForRobotValidation:
				flag = true;
				break;
			case BattleCountdownState.WaitToExit:
				HideWaitScreen();
				break;
			default:
				flag = true;
				break;
			case BattleCountdownState.ConnectedToLobby:
			case BattleCountdownState.WaitingForRejoinAction:
			case BattleCountdownState.ShowingAFKWarning:
				break;
			}
			if (!flag)
			{
				_state = BattleCountdownState.EnteringGame;
				lobbyPresenter.LeftQueueEvent -= HandleOnDeregisteredFromGame;
				afkBlockTimer.onSecondsChanged -= SetBlockedLobbyCountDown;
			}
			else
			{
				RemoteLogger.Error("ENTER GAME ERROR", "Client received enter game message in state " + _state.ToString(), null);
				guiInputController.CloseCurrentScreen();
			}
			return !flag;
		}

		private void HandleOnBlockFromEnteringLobby(long remainingBlockTimeInTicks)
		{
			_afkBlockTimerView.SetAFKBlockPanelVisible(visible: true);
			afkBlockTimer.StartCountUp(remainingBlockTimeInTicks);
			SetBattleCountDownState(BattleCountdownState.ShowingAFKWarning);
		}

		public void HandleRejoinBattleOnUnblocked()
		{
			afkBlockTimer.StopCount();
			afkBlockTimer.onSecondsChanged -= SetBlockedLobbyCountDown;
			ChangeStateToReregistrationRequested();
			_afkBlockTimerView.SetAFKBlockPanelVisible(visible: false);
		}

		public bool Hide()
		{
			bool result = false;
			switch (_state)
			{
			case BattleCountdownState.ConnectingToLobby:
				Console.LogWarning("BattleCountdown screen received Hide signal in ConnectingToLobby State");
				break;
			case BattleCountdownState.ConnectedToLobby:
			case BattleCountdownState.ShowingAFKWarning:
				lobbyPresenter.LeftQueueEvent -= HandleOnDeregisteredFromGame;
				afkBlockTimer.StopCount();
				_view.Hide();
				ShowWaitScreen();
				ChangeStateToWaitToExit();
				break;
			case BattleCountdownState.WaitToExit:
				Console.LogWarning("BattleCountdown screen received Hide signal in WaitToExit State");
				break;
			case BattleCountdownState.WaitingForRobotValidation:
			case BattleCountdownState.Exited:
				_view.Hide();
				ChangeStateToIdle();
				result = true;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case BattleCountdownState.Idle:
			case BattleCountdownState.WaitingForRejoinAction:
			case BattleCountdownState.BattleFound:
			case BattleCountdownState.EnteringGame:
				break;
			}
			return result;
		}

		public void GameFound()
		{
			_state = BattleCountdownState.BattleFound;
			_view.ShowGameFoundAnimation();
		}

		public void GameCancelled()
		{
			_state = BattleCountdownState.ConnectedToLobby;
			_view.ShowPlayerDisconnectedAnimation();
		}

		private void ShowWaitScreen()
		{
			_loadingIcon = gameObjectFactory.Build("GenericLoadingDialog");
			_loadingIcon.set_name("BattleCountDownScreen");
			_loadingIcon.SetActive(true);
			_loadingIcon.GetComponent<GenericLoadingScreen>().text = StringTableBase<StringTable>.Instance.GetString("strContactingServer");
		}

		private void HandleOnDeregisteredFromGame()
		{
			_state = BattleCountdownState.Exited;
			lobbyPresenter.LeftQueueEvent -= HandleOnDeregisteredFromGame;
			_view.Hide();
			afkBlockTimer.onSecondsChanged -= SetBlockedLobbyCountDown;
			HideWaitScreen();
			guiInputController.CloseCurrentScreen();
			if (_registerTimeoutErrorWindowPending)
			{
				_registerTimeoutErrorWindowPending = false;
				DisplayError("strConnectionError", "strUnableConnectToLobby");
			}
			if (_deferredErrorToShow)
			{
				DisplayDeferredError();
			}
		}

		private void HideWaitScreen()
		{
			Object.Destroy(_loadingIcon);
			_loadingIcon = null;
		}

		private void ChangeStateToWaitToExit()
		{
			_state = BattleCountdownState.WaitToExit;
			if (_exitTimeOutCheckTask != null)
			{
				_exitTimeOutCheckTask.Stop();
			}
			_exitTimeOutCheckTask.Start((Action<PausableTaskException>)null, (Action)null);
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			lobbyPresenter.LeftQueueEvent += HandleOnDeregisteredFromGame;
			lobbyPresenter.LeaveMatchmakingQueue();
			afkBlockTimer.onSecondsChanged -= SetBlockedLobbyCountDown;
		}

		private void ChangeStateToIdle()
		{
			_state = BattleCountdownState.Idle;
			lobbyPresenter.LeftQueueEvent -= HandleOnDeregisteredFromGame;
			lobbyCountdown.onSecondsChanged -= UpdateLobbyLabels;
			afkBlockTimer.onSecondsChanged -= SetBlockedLobbyCountDown;
			this.OnBattleCountdownHidden();
			guiInputController.SetShortCutMode(shortCutMode);
		}

		public void SetEstimatedQueueTime(int averageWaitTime)
		{
			_view.SetAverageWaitTime(averageWaitTime);
		}

		public void CloseWindowWithError(string strErrorTitleKey, string strErrorBodyKey, string reason = null, Action onRetry = null)
		{
			OnFail(strErrorTitleKey, strErrorBodyKey, reason, onRetry);
		}

		private void OnFail(string strErrorTitleKey, string strErrorBodyKey, string reason = null, Action onRetry = null)
		{
			DisplayError(strErrorTitleKey, strErrorBodyKey, reason, onRetry);
		}

		private void DisplayError(string strErrorTitleKey, string strErrorBodyKey, string reason = null, Action onRetry = null)
		{
			Console.Log("Defer display of error message");
			_deferredErrorToShow = true;
			_deferredErrorTitleKey = strErrorTitleKey;
			_deferredErrorBody = strErrorBodyKey;
			_deferredErrorReason = reason;
			_deferredErrorRetry = onRetry;
		}

		private void UpdateLobbyLabels(int seconds)
		{
			_view.SetCurrentWaitTime(seconds);
		}

		private void SetBlockedLobbyCountDown(int seconds)
		{
			_afkBlockTimerView.SetAFKBlockedTimeCountdown(seconds);
		}

		private IEnumerator CheckExitTimeout()
		{
			_waitForExitStartTime = Time.get_realtimeSinceStartup();
			while (Time.get_realtimeSinceStartup() - _waitForExitStartTime < 15f)
			{
				yield return null;
			}
			BattleCountdownState state = _state;
			if (state == BattleCountdownState.WaitToExit)
			{
				HideWaitScreen();
				ChangeStateToIdle();
				DisplayError("strConnectionError", "strUnableConnectToLobby");
				RemoteLogger.Error("Timeout disconnecting from lobby", string.Empty, null);
			}
		}

		private void DisplayDeferredError()
		{
			GenericErrorData data = (_deferredErrorRetry == null) ? new GenericErrorData(StringTableBase<StringTable>.Instance.GetString(_deferredErrorTitleKey), StringTableBase<StringTable>.Instance.GetString(_deferredErrorBody) + " \r\n " + StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", _deferredErrorReason)) : new GenericErrorData(StringTableBase<StringTable>.Instance.GetString(_deferredErrorTitleKey), StringTableBase<StringTable>.Instance.GetString(_deferredErrorBody) + " \r\n " + StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", _deferredErrorReason), StringTableBase<StringTable>.Instance.GetString("strOK"), StringTableBase<StringTable>.Instance.GetString("strRetry"), null, _deferredErrorRetry);
			infoDisplay.ShowInfoDialogue(data);
			_deferredErrorToShow = false;
			_deferredErrorReason = null;
			_deferredErrorRetry = null;
		}
	}
}
