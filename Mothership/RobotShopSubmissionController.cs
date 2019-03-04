using Mothership.Garage.Thumbnail;
using Services.Analytics;
using Services.Requests.Interfaces;
using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class RobotShopSubmissionController : IInitialize, IFloatingWidget, IWaitForFrameworkDestruction
	{
		public Action<bool> OnRobotSubmissionViewStateChanged;

		private uint _maxSubmissionCount;

		private uint _playerSubmissionCount;

		private uint _earnings;

		private RobotSubmissionView _submissionView;

		private Texture2D _thumbnail;

		private ThumbnailCreator _thumbnailCreator;

		private TriggerUploadView _triggerUploadView;

		private GarageSlotDependency _currentRobot;

		private TLOG_RobotStatsCalculator_Tencent _robotStatsCalculator;

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal RobotShopObserver robotShopObserver
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopTransactionController controller
		{
			private get;
			set;
		}

		[Inject]
		internal EnterBattleChecker enterBattleChecker
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponOrderManager weaponOrderManager
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal ProfanityFilter profanityFilter
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
		internal RobotSanctionController robotSanctionController
		{
			private get;
			set;
		}

		[Inject]
		private GarageChangedObserver garageObserver
		{
			get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			robotShopObserver.OnRobotDeletedEvent += OnRobotDeleted;
			garageObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			robotShopObserver.OnRobotDeletedEvent -= OnRobotDeleted;
			garageObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public IEnumerator LoadData()
		{
			ILoadRobotShopSubmissionInfos loadRobotShopSubmissionInfosReq = serviceFactory.Create<ILoadRobotShopSubmissionInfos>();
			TaskService<LoadRobotShopSubmissionInfosResult> loadRobotShopSubmissionInfosTS = loadRobotShopSubmissionInfosReq.AsTask();
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadRobotShopSubmissionInfosTS, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			});
			yield return handleTSWithError.GetEnumerator();
			if (loadRobotShopSubmissionInfosTS.succeeded)
			{
				LoadRobotShopSubmissionInfosResult result = loadRobotShopSubmissionInfosTS.result;
				_playerSubmissionCount = result.playerSubmissionCount;
				_maxSubmissionCount = result.maxSubmissionCount;
			}
			ILoadRobotMasterySettingsRequest masteryRequest = serviceFactory.Create<ILoadRobotMasterySettingsRequest>();
			TaskService<RobotMasterySettingsDependency> masteryTask = new TaskService<RobotMasterySettingsDependency>(masteryRequest);
			yield return new HandleTaskServiceWithError(masteryTask, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			if (masteryTask.succeeded)
			{
				_earnings = (uint)masteryTask.result.robitsRewardForCRFRobotCreator;
			}
			ILoadMovementStatsRequest movementStatsRequest = serviceFactory.Create<ILoadMovementStatsRequest>();
			TaskService<MovementStats> movementStatsTask = new TaskService<MovementStats>(movementStatsRequest);
			movementStatsTask.Execute();
			IGetDamageBoostRequest damageboostrequest = serviceFactory.Create<IGetDamageBoostRequest>();
			TaskService<DamageBoostDeserialisedData> damageBoostTask = new TaskService<DamageBoostDeserialisedData>(damageboostrequest);
			damageBoostTask.Execute();
			ILoadPlatformConfigurationRequest loadPlatformConfigRequest = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> loadPlatformConfigTask = loadPlatformConfigRequest.AsTask();
			loadPlatformConfigTask.Execute();
			_robotStatsCalculator = new TLOG_RobotStatsCalculator_Tencent(movementStatsTask.result, damageBoostTask.result, loadPlatformConfigTask.result.UseDecimalSystem);
		}

		public void StartUploadRobot()
		{
			RefreshThumbnail();
			OnUploadRequested();
		}

		public void SetupTriggerUploadView(TriggerUploadView triggerUploadView)
		{
			_triggerUploadView = triggerUploadView;
			_triggerUploadView.OnTriggerUploadRequestedEvent += OnUploadRequested;
		}

		public void SetupRobotSubmissionView(RobotSubmissionView robotSubmissionView)
		{
			_submissionView = robotSubmissionView;
			_submissionView.OnUploadConfirmedEvent += OnUploadConfirmed;
		}

		public void SetupThumbnailCreator(ThumbnailCreator thumbnailCreator)
		{
			_thumbnailCreator = thumbnailCreator;
		}

		private void OnUploadRequested()
		{
			if (_currentRobot.crfId != 0)
			{
				_triggerUploadView.ShowUploadError(StringTableBase<StringTable>.Instance.GetString("strShopSubmissionIsCRFRobot"));
				return;
			}
			if (_playerSubmissionCount >= _maxSubmissionCount)
			{
				_triggerUploadView.ShowUploadError(StringTableBase<StringTable>.Instance.GetString("strShopSubmissionUploadLimitReached"));
				return;
			}
			switch (enterBattleChecker.IsMachineValidForUpload())
			{
			case EnterBattleChecker.MachineValidForUploadResult.ContainsBadge:
				_triggerUploadView.ShowUploadError(StringTableBase<StringTable>.Instance.GetString("strMachineInvalidContainsBadge"));
				break;
			case EnterBattleChecker.MachineValidForUploadResult.InvalidForUpload:
				_triggerUploadView.ShowUploadError(StringTableBase<StringTable>.Instance.GetString("strShopSubmissionMachineInvalid"));
				break;
			default:
				TaskRunner.get_Instance().Run(robotSanctionController.CheckRobotSanction(garagePresenter.CurrentRobotIdentifier.ToString(), delegate(RobotSanctionData sanction)
				{
					TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
				}));
				break;
			}
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
			else
			{
				ShowSubmissionDialog();
			}
		}

		private void ShowError(string error)
		{
			_triggerUploadView.ShowUploadError(error);
		}

		private void ShowSubmissionDialog()
		{
			_thumbnail = _thumbnailCreator.RenderThumbnail(ThumbnailCreator.Positioning.CopyGameCameraTransformation, highQuality: true);
			_submissionView.Show(_currentRobot.name, _thumbnail, _earnings);
			SafeEvent.SafeRaise<bool>(OnRobotSubmissionViewStateChanged, true);
		}

		private void OnUploadConfirmed(string name, string description)
		{
			if (profanityFilter.FilterString(name) != name || profanityFilter.FilterString(description) != description)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strUploadRobotErrorTitle"), StringTableBase<StringTable>.Instance.GetString("strUploadRobotProfanityError"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				return;
			}
			if (SocialInputValidation.DoesStringContainInvalidCharacters(ref name) || SocialInputValidation.DoesStringContainInvalidCharacters(ref description))
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strUploadRobotErrorTitle"), StringTableBase<StringTable>.Instance.GetString("strUploadRobotInvalidCharError"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				return;
			}
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			PlayerRobotStats robotStats_ = _robotStatsCalculator.CalculateRobotStats(machineMap, _currentRobot.uniqueSlotId.ToString(), _currentRobot.masteryLevel);
			UploadRobotDependency param = new UploadRobotDependency((int)_currentRobot.garageSlot, name, description, _thumbnail, robotStats_);
			IUploadRobotRequest uploadRobotRequest = serviceFactory.Create<IUploadRobotRequest, UploadRobotDependency>(param);
			uploadRobotRequest.SetAnswer(new ServiceAnswer<bool>(OnUploadDone, OnUploadFailed));
			uploadRobotRequest.Execute();
		}

		private void RefreshThumbnail()
		{
			_thumbnailCreator.RenderThumbnail(ThumbnailCreator.Positioning.CopyGameCameraTransformation, highQuality: true, _thumbnail);
			_submissionView.UpdateThumbnailTexture(_thumbnail);
		}

		private void OnUploadDone(bool uploadSuccessful)
		{
			if (uploadSuccessful)
			{
				_playerSubmissionCount++;
			}
			else
			{
				string @string = StringTableBase<StringTable>.Instance.GetString("strRobocloudError");
				string string2 = StringTableBase<StringTable>.Instance.GetString("strUnableToCompleteUpload");
				string string3 = StringTableBase<StringTable>.Instance.GetString("strOK");
				ErrorWindow.ShowErrorWindow(new GenericErrorData(@string, string2, string3));
			}
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
		}

		private void OnUploadFailed(ServiceBehaviour fail)
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			ErrorWindow.ShowServiceErrorWindow(fail);
		}

		private void OnRobotDeleted()
		{
			_playerSubmissionCount--;
		}

		public void HandleQuitPressed()
		{
			if (_submissionView.get_gameObject().get_activeSelf())
			{
				_submissionView.DismissDialog();
				SafeEvent.SafeRaise<bool>(OnRobotSubmissionViewStateChanged, false);
			}
		}

		private void CacheCurrentRobot(ref GarageSlotDependency selectedRobot)
		{
			_currentRobot = selectedRobot;
		}

		private IEnumerator HandleAnalytics()
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			TaskService<TiersData> loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>().AsTask();
			yield return loadTiersBandingReq;
			if (!loadTiersBandingReq.succeeded)
			{
				Console.LogError("Log RobotShop Uploaded request failed while retrieving Tiers Banding. " + loadTiersBandingReq.behaviour.exceptionThrown);
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			uint tier = RRAndTiers.ConvertRRToTierIndex(tiersData: loadTiersBandingReq.result, isMegabot: _currentRobot.totalRobotCPU > cpuPower.MaxCpuPower, totalRobotRanking: _currentRobot.totalRobotRanking) + 1;
			LogRobotShopUploadedDependency robotShopUploadedDependency = new LogRobotShopUploadedDependency(tier, _currentRobot.totalRobotCPU, _currentRobot.movementCategories, weaponOrderManager.weaponOrder.GetItemCategories());
			TaskService logRobotShopUploadedRequest = analyticsRequestFactory.Create<ILogRobotShopUploadedRequest, LogRobotShopUploadedDependency>(robotShopUploadedDependency).AsTask();
			yield return logRobotShopUploadedRequest;
			if (!logRobotShopUploadedRequest.succeeded)
			{
				Console.LogError("Log RobotShop Uploaded Request failed. " + logRobotShopUploadedRequest.behaviour.exceptionThrown);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
