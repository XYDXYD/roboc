using Authentication;
using Avatars;
using Robocraft.GUI;
using Services.Analytics;
using Simulation;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal sealed class ClanSeasonRewardScreenController : IGUIDisplay, IWaitForFrameworkDestruction, IComponent
	{
		private ITaskRoutine _animationTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();

		private ClanSeasonRewardDataSource _dataSource;

		private GenericComponentMessage _refreshDataMessage;

		private bool _thereIsSomeRewardToShow;

		private ClanSeasonRewardScreenView _view;

		[Inject]
		internal ClanSeasonRewardScreenLayoutFactory layoutFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
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

		[Inject]
		internal IMultiAvatarLoader avatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver avatarAvailableObserver
		{
			private get;
			set;
		}

		[Inject]
		internal ICurrenciesTracker currenciesTracker
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public bool doesntHideOnSwitch => false;

		public bool hasBackground => false;

		public bool isScreenBlurred => true;

		public GuiScreens screenType => GuiScreens.ClanSeasonRewardScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public void EnableBackground(bool enable)
		{
		}

		public bool Hide()
		{
			_view.Hide();
			return true;
		}

		public bool IsActive()
		{
			return _view.IsActive();
		}

		public GUIShowResult Show()
		{
			_view.Show();
			_view.PlayOpenAnimation();
			return GUIShowResult.Showed;
		}

		internal IEnumerator ValidateSeasonRewardAndLoadData()
		{
			Console.Log("Validate Season Reward");
			loadingIconPresenter.NotifyLoading("ValidateSeasonReward");
			IValidateSeasonRewardsRequest validateSeasonRewardRequest = socialRequestFactory.Create<IValidateSeasonRewardsRequest>();
			validateSeasonRewardRequest.Inject(User.Username);
			TaskService task = new TaskService(validateSeasonRewardRequest);
			yield return task;
			if (task.succeeded)
			{
				Console.Log("Season Reward Validated successfully");
			}
			else
			{
				Console.Log("Failed to validate season reward.");
			}
			yield return LoadSeasonRewardData();
			loadingIconPresenter.NotifyLoadingDone("ValidateSeasonReward");
		}

		private IEnumerator LoadSeasonRewardData()
		{
			yield return _dataSource.RefreshData();
			if (_dataSource.NumberOfDataItemsAvailable(0) > 0)
			{
				_thereIsSomeRewardToShow = true;
				DeepBroadcastDownMessage(_refreshDataMessage);
			}
		}

		internal void SetView(ClanSeasonRewardScreenView view)
		{
			_view = view;
			_refreshDataMessage = new GenericComponentMessage(MessageType.RefreshData, _view.VIEW_NAME, string.Empty);
			InitializeScreen();
		}

		internal void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.ButtonClicked && message.Originator == ClanSeasonRewardScreenComponentNames.CONFIRM_BUTTON_NAME)
			{
				loadingIconPresenter.NotifyLoading("Clans");
				IReclaimSeasonRewardsRequest reclaimSeasonRewardsRequest = socialRequestFactory.Create<IReclaimSeasonRewardsRequest>();
				reclaimSeasonRewardsRequest.Inject(User.Username);
				reclaimSeasonRewardsRequest.SetAnswer(new ServiceAnswer<ReclaimSeasonRewardsResponse>(HandleOnRewardReclaimed, OnReclaimFailed)).Execute();
			}
		}

		internal bool ThereIsSomeRewardToShow()
		{
			return _thereIsSomeRewardToShow;
		}

		private IEnumerator WaitForAnimationToFinish(Action OnComplete)
		{
			while (!_view.CurrentAnimationFinished())
			{
				yield return null;
			}
			OnComplete();
		}

		private void HandleOnRewardReclaimed(ReclaimSeasonRewardsResponse data)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			_view.PlayCloseAnimation();
			_animationTask.SetEnumerator(WaitForAnimationToFinish(OnCloseAnimationFinished)).Start((Action<PausableTaskException>)null, (Action)null);
			currenciesTracker.RefreshWallet(delegate(Wallet wallet)
			{
				TaskRunner.get_Instance().Run(HandleAnalytics(data.robitsRewarded, wallet.RobitsBalance));
			});
		}

		private void OnCloseAnimationFinished()
		{
			Hide();
		}

		private void OnReclaimFailed(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private void InitializeScreen()
		{
			_dataSource = new ClanSeasonRewardDataSource(socialRequestFactory, avatarLoader, avatarAvailableObserver);
			layoutFactory.BuildAll(_view, _dataSource);
		}

		private void DeepBroadcastDownMessage(GenericComponentMessage message)
		{
			_view.DeepBroadcastDownMessage(message);
		}

		public void OnFrameworkDestroyed()
		{
			_dataSource.Dispose();
		}

		private IEnumerator HandleAnalytics(int earnedRobitsAmount, long robitsBalance)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			TaskService logCollectedSeasonRewardRequest = analyticsRequestFactory.Create<ILogCollectedSeasonRewardRequest, int>(earnedRobitsAmount).AsTask();
			yield return logCollectedSeasonRewardRequest;
			if (!logCollectedSeasonRewardRequest.succeeded)
			{
				Console.LogError("Log Collected Season Reward Request failed. " + logCollectedSeasonRewardRequest.behaviour.exceptionThrown);
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(CurrencyType.Robits.ToString(), earnedRobitsAmount, robitsBalance, 0, "SeasonClan", _dataSource.QueryData<string>(7, 0));
			TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
			yield return logPlayerCurrencyEarnedRequest;
			if (!logPlayerCurrencyEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
