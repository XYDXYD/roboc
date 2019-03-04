using Authentication;
using Fabric;
using Game.RoboPass.GUI;
using Game.RoboPass.GUI.Observers;
using Mothership.DailyQuest;
using Mothership.GUI;
using Mothership.GUI.CustomGames;
using Mothership.SinglePlayerCampaign;
using Services;
using Services.Analytics;
using Services.Web.Photon;
using SinglePlayerCampaign.GUI.Mothership.DataTypes;
using SinglePlayerServiceLayer;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class InitialMothershipGUIFlow : IWaitForFrameworkInitialization, IInitialize
	{
		private bool _rewardsMusicPlaying;

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		[Inject]
		protected LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		internal EnterPlanetDialogueController enterPlanetDialogueController
		{
			private get;
			set;
		}

		[Inject]
		protected GaragePresenter garage
		{
			get;
			set;
		}

		[Inject]
		protected IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		[Inject]
		protected FriendInviteManager friendInviteManager
		{
			get;
			set;
		}

		[Inject]
		internal RobotShopRatingController ratingController
		{
			private get;
			set;
		}

		[Inject]
		internal ProfileUpdateNotification profileUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal BundleAwardController bundleAwardController
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal PurchaseRefresher purchaseRefresher
		{
			private get;
			set;
		}

		[Inject]
		internal ClanController clanController
		{
			private get;
			set;
		}

		[Inject]
		internal FriendController friendController
		{
			private get;
			set;
		}

		[Inject]
		protected PremiumMembership premiumMembership
		{
			get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		protected ICubesData cubesData
		{
			get;
			set;
		}

		[Inject]
		internal ClanSeasonRewardScreenController clanSeasonRewardScreenController
		{
			private get;
			set;
		}

		[Inject]
		internal BattleExperiencePresenter battleExperiencePresenter
		{
			private get;
			set;
		}

		[Inject]
		internal BattleExperienceLevelPresenter battleExperienceLevelPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal BrawlButtonPresenter brawlButtonGUIPresenter
		{
			get;
			set;
		}

		[Inject]
		internal BrawlDetailsPresenter brawlDetailsPresenter
		{
			get;
			set;
		}

		[Inject]
		internal IDispatchWorldSwitching dispatchWorldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal AccountSanctionsMothership accountSanctions
		{
			get;
			set;
		}

		[Inject]
		internal InitialLoginAnalytics initialLoginAnalytics
		{
			get;
			set;
		}

		[Inject]
		internal CustomGameScreenController customGameScreenController
		{
			get;
			set;
		}

		[Inject]
		internal CustomGamePartyGUIController customGamePartyGUIController
		{
			get;
			set;
		}

		[Inject]
		internal ChatPresenter chatPresenter
		{
			get;
			set;
		}

		[Inject]
		internal HUDDamageBoostPresenter damageBoostPresenter
		{
			get;
			set;
		}

		[Inject]
		internal HUDHealthPresenter hudHealthPresenter
		{
			get;
			set;
		}

		[Inject]
		internal HUDPlayerLevelPresenter hudPlayerLevelPresenter
		{
			get;
			set;
		}

		[Inject]
		internal HUDCPUPowerGaugePresenter cpuGauge
		{
			get;
			set;
		}

		[Inject]
		internal HUDSpeedPreseneter speedPresenter
		{
			get;
			set;
		}

		[Inject]
		internal TauntsMothershipController tauntsMotherShipController
		{
			get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			get;
			set;
		}

		[Inject]
		internal PrebuiltRobotPresenter prebuiltRobotPresenter
		{
			get;
			set;
		}

		[Inject]
		internal ReconnectPresenter reconnectPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IMothershipPropPresenter mothershipPropPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal DailyQuestController dailyQuestController
		{
			private get;
			set;
		}

		[Inject]
		internal QuestProgressionPresenter questProgressionPresenter
		{
			private get;
			set;
		}

		[Inject]
		public GarageSlotsPresenter garageSlotsPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarSelectionPresenter avatarSelectionPresenter
		{
			get;
			set;
		}

		[Inject]
		internal RobotShopSubmissionController robotShopSubmissionController
		{
			get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal TierProgressionRewardPresenter tierProgressionRewardPresenter
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
		internal RoboPassBattleSummaryObserver roboPassBattleSummaryObserver
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
		internal TechPointsPresenter techPointsPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal ISinglePlayerRequestFactory spRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IEntityFactory entityFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ShowCampaignCompleteScreenEngine campaignCompleteScreen
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
		internal AwardedItemsController awardedItemsController
		{
			private get;
			set;
		}

		[Inject]
		internal RealMoneyStorePresenter realMoneyStorePresenter
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

		[Inject]
		internal TLOG_LeftEditModeTracker_Tencent leftEditModeToGarageTracker
		{
			get;
			set;
		}

		[Inject]
		internal MothershipReadyObservable mothershipReadyObservable
		{
			get;
			set;
		}

		public void OnDependenciesInjected()
		{
			loadingIconPresenter.forceOpaque = true;
		}

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)(() => InitialDisplay(WorldSwitching.IsInBuildMode())));
		}

		protected virtual IEnumerator InitialDisplay(bool editMode)
		{
			yield return (object)new WaitForEndOfFrame();
			bool isNewSession = PlayerPrefs.GetInt("NewSession") == 1;
			if (isNewSession)
			{
				PlayerPrefs.SetInt("NewSession", 0);
				yield return LogLoadingRequest(isNewSession, "InitialMothershipGuiFlowStart");
				initialLoginAnalytics.SendLoggedInEvents();
			}
			EnableLoadingScreen();
			currenciesTracker.RefreshWallet();
			worldSwitching.SetAdditionaLoadingScreenMessage(null);
			guiInputController.ShowScreen(GuiScreens.Garage);
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			yield return accountSanctions.RefreshData();
			ISetNewInventoryCubesRequest setNewCubes = serviceFactory.Create<ISetNewInventoryCubesRequest>();
			setNewCubes.Inject(new HashSet<uint>());
			setNewCubes.Execute();
			yield return cpuPower.LoadData();
			yield return premiumMembership.Initialize();
			yield return clanSeasonRewardScreenController.ValidateSeasonRewardAndLoadData();
			yield return clanController.LoadData();
			yield return enterPlanetDialogueController.LoadData();
			yield return robotShopSubmissionController.LoadData();
			yield return battleExperiencePresenter.InitializeInFlow();
			yield return BrawlOverridePreloader.LoadBrawlLanguageStringOverrides(serviceFactory, loadingIconPresenter);
			yield return brawlButtonGUIPresenter.LoadGUIData();
			yield return brawlDetailsPresenter.LoadGUIData();
			yield return battleExperienceLevelPresenter.LoadGUIData();
			yield return customGameScreenController.LoadGUIData();
			yield return customGamePartyGUIController.LoadGUIData();
			yield return chatPresenter.InitializeInFlow();
			yield return tauntsMotherShipController.Initialise();
			yield return prebuiltRobotPresenter.LoadData();
			yield return garageSlotsPresenter.LoadData();
			yield return mothershipPropPresenter.LoadInitialState();
			yield return cubeInventory.RefreshAndWait();
			leftEditModeToGarageTracker.LoadData();
			yield return CreateCampaignDataEntity();
			DisableLoadingScreen();
			yield return purchaseRefresher.RequestDataAsTask();
			EnableLoadingScreen();
			yield return InitialiseRobotInfoPanel();
			Console.Log("Waiting for garage loading");
			while (!garage.isReady)
			{
				yield return null;
			}
			garage.BuildInitialRobot();
			Console.Log("Waiting for garage building complete");
			while (garage.isBusyBuilding)
			{
				yield return null;
			}
			Localization.onLocalize.Invoke();
			DisableLoadingScreen();
			yield return LogLoadingRequest(isNewSession, "ShowRewardScreensStart");
			yield return ShowRewardScreens(editMode);
			yield return LogLoadingRequest(isNewSession, "ShowRewardScreensEnd");
			yield return awardedItemsController.GetAwardedCubeDataAndShowScreen();
			StopRewardsMusic();
			Console.Log("all screens were dismissed");
			EnableLoadingScreen();
			friendController.Show();
			guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			friendInviteManager.MothershipFlowCompleted();
			guiInputController.MothershipFlowCompleted();
			cpuPower.MothershipFlowCompleted();
			hudPlayerLevelPresenter.MothershipFlowCompleted();
			accountSanctions.MothershipFlowCompleted();
			yield return robotSanctionController.CheckAllRobotsSanctions();
			if (dispatchWorldSwitching.SwitchingFrom == WorldSwitchMode.SimulationMP)
			{
				DisableLoadingScreen();
				loadingIconPresenter.forceOpaque = false;
				Console.Log("Waiting for rating controller active");
				yield return ratingController.ConditionalShowRateYourRobotPopup();
				EnableLoadingScreen();
				loadingIconPresenter.forceOpaque = true;
			}
			yield return LogLoadingRequest(isNewSession, "InitialMothershipGuiFlowEnd");
			yield return GetPlatoonData();
			DisableLoadingScreen();
			Console.BatchLog = true;
			Console.Log("Mothership Started");
			worldSwitching.RestoreAvatarPosition();
			reconnectPresenter.CheckReconnection();
			loadingIconPresenter.forceOpaque = false;
			mothershipReadyObservable.Dispatch();
		}

		private IEnumerator CreateCampaignDataEntity()
		{
			loadingIconPresenter.NotifyLoading("LoadingSinglePlayerCampaigns");
			ILoadSinglePlayerCampaignsRequest request = spRequestFactory.Create<ILoadSinglePlayerCampaignsRequest>();
			TaskService<GetCampaignsRequestResult> task = new TaskService<GetCampaignsRequestResult>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingSinglePlayerCampaigns");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingSinglePlayerCampaigns");
			}).GetEnumerator();
			if (!task.succeeded)
			{
				throw new Exception("Failed to load campaigns.");
			}
			CampaignDataImplementor campaignDataImplementor = new CampaignDataImplementor(task.result.CampaignsGameParameters);
			entityFactory.BuildEntity<CampaignDataEntityDescriptor>(205, new object[1]
			{
				campaignDataImplementor
			});
			loadingIconPresenter.NotifyLoadingDone("LoadingSinglePlayerCampaigns");
		}

		protected IEnumerator InitialiseRobotInfoPanel()
		{
			hudHealthPresenter.Initialise();
			damageBoostPresenter.Initialise();
			cpuGauge.Initialise();
			ParallelTaskCollection parallelTaskCollection = new ParallelTaskCollection(3, (string)null);
			parallelTaskCollection.Add(speedPresenter.LoadGUIData());
			parallelTaskCollection.Add(damageBoostPresenter.LoadGUIData());
			parallelTaskCollection.Add(hudPlayerLevelPresenter.LoadGUIData());
			yield return parallelTaskCollection;
		}

		protected void EnableLoadingScreen()
		{
			loadingIconPresenter.NotifyLoading("InitialMothershipGUIFlow");
		}

		protected void DisableLoadingScreen()
		{
			loadingIconPresenter.NotifyLoadingDone("InitialMothershipGUIFlow");
		}

		private IEnumerator GetPlatoonData()
		{
			IGetPlatoonDataRequest getPlatoonDataReq = socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTaskService = new TaskService<Platoon>(getPlatoonDataReq);
			Console.Log("Getting platoon data");
			HandleTaskServiceWithError handleTaskServiceWithError = new HandleTaskServiceWithError(getPlatoonTaskService, delegate
			{
				loadingIconPresenter.NotifyLoading("GetPlatoonData");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("GetPlatoonData");
			});
			yield return handleTaskServiceWithError.GetEnumerator();
			if (getPlatoonTaskService.succeeded)
			{
				Console.Log("Platoon data obtained");
				Platoon result = getPlatoonTaskService.result;
				if (result.isInPlatoon)
				{
					ResetPlatoonMemberStatus(result);
				}
			}
		}

		private void ResetPlatoonMemberStatus(Platoon platoon)
		{
			string username = User.Username;
			SetPlatoonMemberStatusDependency param = new SetPlatoonMemberStatusDependency(username, PlatoonMember.MemberStatus.Ready);
			ISetPlatoonMemberStatusRequest setPlatoonMemberStatusRequest = socialRequestFactory.Create<ISetPlatoonMemberStatusRequest, SetPlatoonMemberStatusDependency>(param);
			ServiceAnswer answer = new ServiceAnswer(LogServiceSuccess, LogServiceFailed);
			IServiceRequest serviceRequest = setPlatoonMemberStatusRequest.SetAnswer(answer);
			serviceRequest.Execute();
		}

		private void LogServiceSuccess()
		{
			Console.Log("Platoon member status successfully set");
		}

		private void LogServiceFailed(ServiceBehaviour serviceBehaviour)
		{
			Console.LogException(serviceBehaviour.exceptionThrown);
		}

		private IEnumerator ShowRewardScreens(bool editMode)
		{
			Console.Log("showing rewards screens");
			yield return ShowBattleExperienceScreen();
			yield return ShowCampaignBadgeScreen();
			yield return ShowTierRankScreen();
			yield return ShowDailyQuestsScreen();
			yield return ShowRoboPassRewards();
			yield return ShowClanSeasonRewardScreen();
			yield return ShowTechPointsScreen(editMode);
			yield return ShowPendingNotification();
		}

		private IEnumerator ShowBattleExperienceScreen()
		{
			if (battleExperiencePresenter.HasExperienceToShow())
			{
				PlayRewardsMusic();
				guiInputController.ShowScreen(GuiScreens.BattleExperienceScreen);
				while (battleExperiencePresenter.IsActive() || buyPremiumAfterBattlePresenter.IsActive())
				{
					yield return null;
				}
			}
		}

		private IEnumerator ShowCampaignBadgeScreen()
		{
			TaskService<LastCompletedCampaign?> getLastCompletedCampaignTaskService = spRequestFactory.Create<IGetLastCompletedCampaignRequest>().AsTask();
			loadingIconPresenter.NotifyLoading("GetLastCompletedCampaign");
			yield return new HandleTaskServiceWithError(getLastCompletedCampaignTaskService, delegate
			{
				loadingIconPresenter.NotifyLoading("GetLastCompletedCampaign");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("GetLastCompletedCampaign");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("GetLastCompletedCampaign");
			if (!getLastCompletedCampaignTaskService.succeeded)
			{
				throw new Exception("GetLastCompletedCampaignRequest failed.");
			}
			if (getLastCompletedCampaignTaskService.result.HasValue)
			{
				LastCompletedCampaign value = getLastCompletedCampaignTaskService.result.Value;
				string campaignId = value.campaignId;
				LastCompletedCampaign value2 = getLastCompletedCampaignTaskService.result.Value;
				LastCompletedCampaignImplementor lastCompletedCampaignImplementor = new LastCompletedCampaignImplementor(campaignId, value2.difficulty);
				entityFactory.BuildEntity<LastCompletedCampaignEntityDescriptor>(206, new object[1]
				{
					lastCompletedCampaignImplementor
				});
				yield return null;
				PlayRewardsMusic();
				guiInputController.ShowScreen(GuiScreens.CampaignComplete);
				while (campaignCompleteScreen.IsActive())
				{
					yield return null;
				}
				TaskService markLastCompletedCampaignAsSeenTaskService = spRequestFactory.Create<IMarkLastCompletedCampaignAsSeenRequest>().AsTask();
				loadingIconPresenter.NotifyLoading("MarkLastCompletedCampaignAsSeen");
				yield return new HandleTaskServiceWithError(markLastCompletedCampaignAsSeenTaskService, delegate
				{
					loadingIconPresenter.NotifyLoading("MarkLastCompletedCampaignAsSeen");
				}, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("MarkLastCompletedCampaignAsSeen");
				}).GetEnumerator();
				loadingIconPresenter.NotifyLoadingDone("MarkLastCompletedCampaignAsSeen");
				if (!markLastCompletedCampaignAsSeenTaskService.succeeded)
				{
					throw new Exception("MarkLastCompletedCampaignAsSeenRequest failed.");
				}
			}
		}

		private IEnumerator ShowTierRankScreen()
		{
			yield return tierProgressionRewardPresenter.LoadDataAndShowRewards();
			if (tierProgressionRewardPresenter.IsActive())
			{
				PlayRewardsMusic();
			}
			while (tierProgressionRewardPresenter.IsActive())
			{
				yield return null;
			}
		}

		private IEnumerator ShowDailyQuestsScreen()
		{
			yield return dailyQuestController.LoadData();
			if (dailyQuestController.playerQuestData != null && questProgressionPresenter.HasQuestProgression())
			{
				questProgressionPresenter.Show();
				while (questProgressionPresenter.IsActive())
				{
					yield return null;
				}
				yield return dailyQuestController.ReceiveQuestRewards();
				yield return dailyQuestController.LoadData();
				yield return currenciesTracker.RefreshUserWalletEnumerator();
				playerLevelNeedRefreshObservable.Dispatch();
			}
		}

		private IEnumerator ShowRoboPassRewards()
		{
			WaitForRoboPassBattleSummaryScreenDismissedTask dismissedTask = new WaitForRoboPassBattleSummaryScreenDismissedTask(roboPassBattleSummaryObserver);
			guiInputController.ShowScreen(GuiScreens.RoboPassBattleSummaryScreen);
			yield return dismissedTask;
			guiInputController.ForceCloseJustThisScreen(GuiScreens.RoboPassBattleSummaryScreen);
		}

		private IEnumerator ShowClanSeasonRewardScreen()
		{
			if (clanSeasonRewardScreenController.ThereIsSomeRewardToShow())
			{
				PlayRewardsMusic();
				guiInputController.ShowScreen(GuiScreens.ClanSeasonRewardScreen);
				while (clanSeasonRewardScreenController.IsActive())
				{
					yield return null;
				}
			}
		}

		private IEnumerator ShowTechPointsScreen(bool editMode)
		{
			if (!editMode)
			{
				yield return techPointsPresenter.ShowTechPointAwards();
			}
		}

		private IEnumerator ShowPendingNotification()
		{
			while (profileUpdater.ShowPendingNotification())
			{
				Console.LogWarning("Waiting for profiler updater ready");
				while (!profileUpdater.isReady)
				{
					yield return null;
				}
			}
		}

		private IEnumerator LogLoadingRequest(bool isNewSession, string reason)
		{
			if (isNewSession)
			{
				TaskService logLoadingRequest = analyticsRequestFactory.Create<ILogLoadingRequest, string>(reason).AsTask();
				yield return logLoadingRequest;
				if (!logLoadingRequest.succeeded)
				{
					Console.LogError("Log Loading Request failed. " + logLoadingRequest.behaviour.exceptionThrown);
				}
			}
		}

		private void PlayRewardsMusic()
		{
			if (!_rewardsMusicPlaying)
			{
				_rewardsMusicPlaying = true;
				EventManager.get_Instance().PostEvent("MUS_Menu_loop", 2);
				EventManager.get_Instance().PostEvent("MUS_RewardScreen", 0);
			}
		}

		private void StopRewardsMusic()
		{
			if (_rewardsMusicPlaying)
			{
				_rewardsMusicPlaying = false;
				EventManager.get_Instance().PostEvent("MUS_RewardScreen", 1);
				EventManager.get_Instance().PostEvent("MUS_Menu_loop", 3);
			}
		}
	}
}
