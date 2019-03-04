using Authentication;
using Robocraft.GUI;
using Services.Analytics;
using Services.Requests.Interfaces;
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
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class BattleExperiencePresenter : IGUIDisplay, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IComponent
	{
		private readonly ITaskRoutine _animationTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();

		private readonly ITaskRoutine _waitForEnterPressedTR;

		private readonly ITaskRoutine _restartFlowTask;

		private BattleExperienceDataSource _dataSource;

		private BattleExperienceView _view;

		private bool _clanInfoFetched;

		private bool _isReady;

		private bool _canBuyPremium;

		[Inject]
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
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
		internal BattleExperienceScreenFactory factory
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
		internal PremiumMembership premiumMembership
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

		[Inject]
		internal PlayerLevelNeedRefreshObservable playerLevelNeedRefreshObservable
		{
			private get;
			set;
		}

		[Inject]
		internal BuyPremiumAfterBattlePresenter buyPremiumAfterBattlePresenter
		{
			private get;
			set;
		}

		public HudStyle battleHudStyle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool doesntHideOnSwitch => true;

		public bool hasBackground => false;

		public bool isScreenBlurred => true;

		public GuiScreens screenType => GuiScreens.BattleExperienceScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public BattleExperiencePresenter()
		{
			_waitForEnterPressedTR = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
			_restartFlowTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)RefreshDataAndScreen);
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return _view != null && _view.get_gameObject().get_activeSelf();
		}

		public GUIShowResult Show()
		{
			_waitForEnterPressedTR.Start((Action<PausableTaskException>)null, (Action)null);
			_view.Show(premiumMembership.hasSubscription, _canBuyPremium);
			_view.PlayShowAnimation();
			_view.BroadcastDownMessage(default(ShowBattleExperienceMessage));
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_view.Hide();
			return true;
		}

		public void SetView(BattleExperienceView view)
		{
			_view = view;
		}

		private void SetHasClan(bool hasClan)
		{
			_view.ShownWithClan(hasClan);
			_view.ShownWithoutClan(hasClan);
		}

		public bool IsReady()
		{
			return _isReady && premiumMembership.Loaded() && _clanInfoFetched;
		}

		public bool HasExperienceToShow()
		{
			return _dataSource.NumberOfDataItemsAvailable(0) > 0;
		}

		public void OnFrameworkInitialized()
		{
			premiumMembership.onSubscriptionActivated += HandlePremiumMembershipSubscriptionActivated;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadBuyPremiumAvailability);
			BuildView();
		}

		private IEnumerator LoadBuyPremiumAvailability()
		{
			ILoadPlatformConfigurationRequest platformConfigRequest = serviceRequestFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> platformConfigTask = platformConfigRequest.AsTask();
			yield return platformConfigTask;
			if (platformConfigTask.succeeded)
			{
				_canBuyPremium = platformConfigTask.result.BuyPremiumAvailable;
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(platformConfigTask.behaviour);
			}
		}

		public void OnFrameworkDestroyed()
		{
			premiumMembership.onSubscriptionActivated -= HandlePremiumMembershipSubscriptionActivated;
		}

		private void HandlePremiumMembershipSubscriptionActivated(TimeSpan duration)
		{
			if (IsActive() && premiumMembership.hasSubscription)
			{
				_restartFlowTask.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private void BuildView()
		{
			_dataSource = new BattleExperienceDataSource(socialRequestFactory, serviceRequestFactory);
			factory.BuildAll(_view, _dataSource);
		}

		public IEnumerator InitializeInFlow()
		{
			loadingIconPresenter.NotifyLoading("BattleExperience");
			IGetMyClanInfoRequest getClanRequest = socialRequestFactory.Create<IGetMyClanInfoRequest>();
			TaskService<ClanInfo> getClanTask = new TaskService<ClanInfo>(getClanRequest);
			yield return new HandleTaskServiceWithError(getClanTask, delegate
			{
				loadingIconPresenter.NotifyLoading("BattleExperience");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			}).GetEnumerator();
			if (getClanTask.succeeded)
			{
				ClanInfo result = getClanTask.result;
				SetHasClan(result != null);
				_clanInfoFetched = true;
			}
			else
			{
				SetHasClan(hasClan: false);
				_clanInfoFetched = true;
			}
			yield return _dataSource.RefreshData();
			bool shouldShowTierBonus = _dataSource.QueryData<bool>(22, 0);
			_view.SetTierBonusActive(shouldShowTierBonus);
			BattleExperienceDataSource.LongPlayWarningMessageType longPlayWarning = _dataSource.QueryData<BattleExperienceDataSource.LongPlayWarningMessageType>(21, 0);
			switch (longPlayWarning)
			{
			case BattleExperienceDataSource.LongPlayWarningMessageType.NoWarning:
				_view.HideLongPlayWarningElements();
				break;
			case BattleExperienceDataSource.LongPlayWarningMessageType.Warning_50Percent:
				_view.ShowLongPlayWarningElements(longPlayWarning);
				break;
			case BattleExperienceDataSource.LongPlayWarningMessageType.Warning_100Percent:
				_view.ShowLongPlayWarningElements(longPlayWarning);
				break;
			}
			_dataSource.SetPremiumState(premiumMembership.hasSubscription);
			_view.BroadcastDownMessage(new GenericComponentMessage(MessageType.RefreshData, "BattleExperienceRoot", string.Empty));
			_isReady = true;
			loadingIconPresenter.NotifyLoadingDone("BattleExperience");
		}

		public void PurchasePremiumPressed()
		{
			PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "BattleSummary", startsNewChain: true);
			guiInputController.ToggleScreen(GuiScreens.BuyPremiumAfterBattle);
		}

		internal void OnAskClose()
		{
			loadingIconPresenter.NotifyLoading("BattleExperience");
			_waitForEnterPressedTR.Stop();
			TaskRunner.get_Instance().Run(CollectRewards());
		}

		private IEnumerator CollectRewards()
		{
			ICollectPreviousBattleRewardsRequest collectPreviousBattleRewardsRequest = socialRequestFactory.Create<ICollectPreviousBattleRewardsRequest>();
			collectPreviousBattleRewardsRequest.Inject(User.Username);
			TaskService<bool> serviceTask = collectPreviousBattleRewardsRequest.AsTask();
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(serviceTask, delegate
			{
				loadingIconPresenter.NotifyLoading("BattleExperience");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			});
			yield return handleTSWithError.GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			if (serviceTask.succeeded)
			{
				Console.Log("set seen previous battle rewards: " + serviceTask.result.ToString());
				currenciesTracker.RefreshWallet(delegate(Wallet wallet)
				{
					TaskRunner.get_Instance().Run(HandleAnalytics(wallet.RobitsBalance));
				});
			}
			StartClosing();
		}

		private void StartClosing()
		{
			_restartFlowTask.Stop();
			guiInputController.ForceCloseJustThisScreen(GuiScreens.BuyPremiumAfterBattle);
			_view.PlayHideAnimation();
			_animationTask.SetEnumerator(WaitForAnimationToFinish(OnExitAnimationFinished)).Start((Action<PausableTaskException>)null, (Action)null);
			playerLevelNeedRefreshObservable.Dispatch();
		}

		private void OnExitAnimationFinished()
		{
			guiInputController.ForceCloseJustThisScreen(GuiScreens.BattleExperienceScreen);
		}

		private IEnumerator WaitForAnimationToFinish(Action OnComplete)
		{
			while (_view.IsAnimationPlaying())
			{
				yield return null;
			}
			OnComplete();
		}

		private IEnumerator HandleAnalytics(long robitsBalance)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			int previousLevel = _dataSource.QueryData<int>(7, 0) - _dataSource.QueryData<int>(8, 0);
			string source = PlayerPrefs.GetString("analytics_gameMode_" + User.Username);
			if (string.IsNullOrEmpty(source))
			{
				source = "BattleUnknown";
			}
			string sourceDetail = PlayerPrefs.GetString("analytics_levelName_" + User.Username);
			PlayerPrefs.DeleteKey("analytics_gameMode_" + User.Username);
			PlayerPrefs.DeleteKey("analytics_levelName_" + User.Username);
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceRequestFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			loadPlayerRoboPassSeasonReq.ClearCache();
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			if (!loadPlayerRoboPassSeasonTS.succeeded)
			{
				Console.LogError("Failed to get RoboPass player season data");
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			int? roboPassXP = null;
			if (playerRoboPassSeasonData != null)
			{
				roboPassXP = playerRoboPassSeasonData.xpFromSeasonStart + playerRoboPassSeasonData.deltaXpToShow;
			}
			LogPlayerXpEarnedDependency playerXpEarnedDependency = new LogPlayerXpEarnedDependency(_dataSource.QueryData<int>(5, 0), _dataSource.QueryData<int>(6, 0), roboPassXP, previousLevel, _dataSource.QueryData<int>(2, 0), source, sourceDetail);
			TaskService logPlayerXpEarnedRequest = analyticsRequestFactory.Create<ILogPlayerXpEarnedRequest, LogPlayerXpEarnedDependency>(playerXpEarnedDependency).AsTask();
			yield return logPlayerXpEarnedRequest;
			if (!logPlayerXpEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Xp Request failed. " + logPlayerXpEarnedRequest.behaviour.exceptionThrown);
			}
			int premiumRobits = premiumMembership.hasSubscription ? _dataSource.QueryData<int>(19, 0) : 0;
			LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(earned_: _dataSource.QueryData<int>(18, 0) + premiumRobits, currency_: CurrencyType.Robits.ToString(), balance_: robitsBalance, premiumBonus_: premiumRobits, source_: source, sourceDetail_: sourceDetail);
			TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
			yield return logPlayerCurrencyEarnedRequest;
			if (!logPlayerCurrencyEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator Update()
		{
			while (true)
			{
				if (Input.GetKeyDown(13))
				{
					OnAskClose();
				}
				yield return null;
			}
		}

		private IEnumerator RefreshDataAndScreen()
		{
			loadingIconPresenter.NotifyLoading("BattleExperience");
			yield return _dataSource.RefreshData();
			factory.BuildAll(_view, _dataSource);
			loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			while (buyPremiumAfterBattlePresenter.IsActive())
			{
				yield return null;
			}
			yield return InitializeInFlow();
			Show();
		}
	}
}
