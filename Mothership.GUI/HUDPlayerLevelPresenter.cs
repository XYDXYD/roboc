using Fabric;
using Services.Analytics;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class HUDPlayerLevelPresenter : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private const float INTERP_SPEED_FACTOR = 0.5f;

		private const float INTERP_STOP_THRESHOLD = 0.001f;

		private const string AUDIO_EVENT_NAME_LEVEL_UP = "GUI_Ingame_LevelUP";

		private readonly ITaskRoutine _trackPlayerInactivityTaskRoutine;

		private readonly ITaskRoutine _trackTimeToGenXPTaskRoutine;

		private HUDPlayerLevelView _hudPlayerLevelView;

		private bool _areRoutinesRunning;

		private bool _isMothershipFlowCompleted;

		private float _periodEarnXP;

		private float _periodUserInactivity;

		private float _playerProgressCurrent;

		private float _timerGenXP;

		private float _timerPlayerInactivity;

		private int _currentPlayerLevel;

		private int _startingPlayerLevel;

		private int _totalEarnedXP;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache1;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder machineBuilder
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
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
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
		internal TechPointsPresenter techPointsPresenter
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
		internal SwitchingToTestModeObserver switchingToTestModeObserver
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

		[Inject]
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		public HUDPlayerLevelPresenter()
		{
			_trackPlayerInactivityTaskRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)TrackInactivityTimer);
			_trackTimeToGenXPTaskRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)TrackTimeToGenerateXP);
		}

		public void OnFrameworkInitialized()
		{
			guiInputController.OnScreenStateChange += OnScreenStateChange;
			machineBuilder.OnPlaceCube += HandleCubePlacedOrRemoved;
			machineBuilder.OnDeleteCube += HandleCubePlacedOrRemoved;
			colorUpdater.OnCubePainted += ResetTimerAndStartRoutines;
			worldSwitching.OnWorldJustSwitched += OnWorldSwitched;
		}

		public void OnFrameworkDestroyed()
		{
			guiInputController.OnScreenStateChange -= OnScreenStateChange;
			machineBuilder.OnPlaceCube -= HandleCubePlacedOrRemoved;
			machineBuilder.OnDeleteCube -= HandleCubePlacedOrRemoved;
			colorUpdater.OnCubePainted -= ResetTimerAndStartRoutines;
			worldSwitching.OnWorldJustSwitched -= OnWorldSwitched;
		}

		internal void HandleCubePlacedOrRemoved(InstantiatedCube cube)
		{
			ResetTimerAndStartRoutines();
		}

		internal void MothershipFlowCompleted()
		{
			_isMothershipFlowCompleted = true;
		}

		internal void SetView(HUDPlayerLevelView hudPlayerLevelView)
		{
			_hudPlayerLevelView = hudPlayerLevelView;
		}

		internal IEnumerator LoadGUIData()
		{
			loadingIconPresenter.NotifyLoading("PlayerLevelInfo");
			IGetBuildingXPSettingsRequest getBuildingXpSettingsReq = serviceRequestFactory.Create<IGetBuildingXPSettingsRequest>();
			TaskService<BuildingXPSettingsDependency> getBuildingXpSettingsTS = new TaskService<BuildingXPSettingsDependency>(getBuildingXpSettingsReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(getBuildingXpSettingsTS, delegate
			{
				loadingIconPresenter.NotifyLoading("PlayerLevelInfo");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("PlayerLevelInfo");
			});
			yield return handleTSWithError.GetEnumerator();
			if (getBuildingXpSettingsTS.succeeded)
			{
				BuildingXPSettingsDependency result = getBuildingXpSettingsTS.result;
				_periodEarnXP = result.buildModePeriodUserEarnXP;
				_periodUserInactivity = result.buildModePeriodUserInactivity;
			}
			yield return PlayerLevelHelper.LoadCurrentPlayerLevel(serviceRequestFactory, SavePlayerData, LogError);
			loadingIconPresenter.NotifyLoadingDone("PlayerLevelInfo");
		}

		internal void DisablePlayerLevelHUDAndXPTracking()
		{
			if (guiInputController.GetActiveScreen() != GuiScreens.BuildMode)
			{
				StopRoutines();
				_hudPlayerLevelView.HideUI();
			}
		}

		private IEnumerator OnWorldSwitching()
		{
			if (worldSwitching.CurrentWorld == WorldSwitchMode.BuildMode)
			{
				yield return HandleAnalytics();
			}
		}

		private void OnWorldSwitched(WorldSwitchMode currentMode)
		{
			worldSwitching.OnWorldIsSwitching.Add(OnWorldSwitching());
			if (_isMothershipFlowCompleted && currentMode == WorldSwitchMode.GarageMode && worldSwitching.SwitchingFrom == WorldSwitchMode.BuildMode)
			{
				TaskRunner.get_Instance().Run(techPointsPresenter.ShowTechPointAwards());
			}
		}

		private void ResetTimerAndStartRoutines()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.BuildMode && !guiInputController.IsTutorialScreenActive())
			{
				_timerPlayerInactivity = 0f;
				StartRoutines();
			}
		}

		private static void LogError()
		{
			Console.LogError("Failed to load player level data!");
		}

		private void SavePlayerData(PlayerLevelAndProgress playerLevelAndProgress)
		{
			_startingPlayerLevel = Convert.ToInt32(playerLevelAndProgress.playerLevel);
			_totalEarnedXP = 0;
			_currentPlayerLevel = _startingPlayerLevel;
			_hudPlayerLevelView.SetPlayerLevel(_currentPlayerLevel);
			_playerProgressCurrent = playerLevelAndProgress.progressToNextLevel;
			_hudPlayerLevelView.SetPlayerProgress(_playerProgressCurrent);
		}

		private void OnScreenStateChange()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateVisibility);
		}

		private IEnumerator UpdateVisibility()
		{
			HUDPlayerLevelView.StyleVersion style = HUDPlayerLevelView.StyleVersion.EditMode;
			IRetrieveCustomGameSessionRequest request = serviceRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> task = new TaskService<RetrieveCustomGameSessionRequestData>(request);
			yield return task;
			if (task.succeeded && task.result.Data != null)
			{
				style = HUDPlayerLevelView.StyleVersion.EditCustomGame;
			}
			if (ShouldShow())
			{
				yield return LoadGUIData();
			}
			if (ShouldShow())
			{
				StartRoutines();
				_hudPlayerLevelView.ShowUI(style);
				yield break;
			}
			StopRoutines();
			if (_hudPlayerLevelView.get_isActiveAndEnabled())
			{
				_hudPlayerLevelView.HideUI();
			}
		}

		private bool ShouldShow()
		{
			return guiInputController.GetActiveScreen() == GuiScreens.BuildMode && !guiInputController.IsTutorialScreenActive();
		}

		private IEnumerator TrackTimeToGenerateXP()
		{
			_timerGenXP = 0f;
			while (true)
			{
				yield return null;
				_timerGenXP += Time.get_deltaTime();
				if (!(_timerGenXP < _periodEarnXP))
				{
					_timerGenXP = 0f;
					ISavePlayerBuildingXPRequest savePlayerBuildingXPRequest = serviceRequestFactory.Create<ISavePlayerBuildingXPRequest>();
					ServiceAnswer<PlayerLevelAndXPData> serviceAnswer = new ServiceAnswer<PlayerLevelAndXPData>(OnSavePlayerXPReqSuccess, OnSavePlayerXPReqFail);
					IServiceRequest serviceRequest = savePlayerBuildingXPRequest.SetAnswer(serviceAnswer);
					serviceRequest.Execute();
				}
			}
		}

		private static void OnSavePlayerXPReqFail(ServiceBehaviour serviceBehaviour)
		{
			Console.LogError("Request failed! Error: " + serviceBehaviour.errorTitle + " " + serviceBehaviour.errorBody);
		}

		private IEnumerator TrackInactivityTimer()
		{
			_timerPlayerInactivity = 0f;
			while (true)
			{
				yield return null;
				_timerPlayerInactivity += Time.get_deltaTime();
				if (_timerPlayerInactivity >= _periodUserInactivity)
				{
					StopRoutines();
				}
			}
		}

		private void OnSavePlayerXPReqSuccess(PlayerLevelAndXPData playerLevelAndXPData)
		{
			int playerGainedXP = playerLevelAndXPData.PlayerGainedXP;
			if (playerGainedXP != 0)
			{
				int playerLevel = playerLevelAndXPData.PlayerLevel;
				playerLevelNeedRefreshObservable.Dispatch();
				TaskRunner.get_Instance().Run(ProgressTweenFilling(playerLevelAndXPData.PlayerProgress));
				_hudPlayerLevelView.ShowXPIncrement(playerGainedXP);
				if (playerLevel > _currentPlayerLevel)
				{
					EventManager.get_Instance().PostEvent("GUI_Ingame_LevelUP", 0);
					_hudPlayerLevelView.SetPlayerLevel(playerLevel);
					_hudPlayerLevelView.ShowLevelUpAnimations();
				}
				_totalEarnedXP += playerGainedXP;
				_currentPlayerLevel = playerLevel;
			}
		}

		private IEnumerator ProgressTweenFilling(float playerProgress)
		{
			float playerProgressCurrent = _playerProgressCurrent;
			if (playerProgress < playerProgressCurrent)
			{
				playerProgressCurrent = 0f;
			}
			bool isTweeningFinished = false;
			float interpFactor = 0f;
			while (!isTweeningFinished)
			{
				yield return null;
				interpFactor += 0.5f * Time.get_deltaTime();
				if (interpFactor > 1f)
				{
					interpFactor = 1f;
				}
				if (playerProgress - playerProgressCurrent > 0.001f)
				{
					playerProgressCurrent = Mathf.Lerp(playerProgressCurrent, playerProgress, interpFactor);
				}
				else
				{
					playerProgressCurrent = playerProgress;
					isTweeningFinished = true;
				}
				playerLevelNeedRefreshObservable.Dispatch();
				_hudPlayerLevelView.SetPlayerProgress(playerProgressCurrent);
			}
			_playerProgressCurrent = playerProgress;
		}

		private void StartRoutines()
		{
			if (!_areRoutinesRunning)
			{
				_areRoutinesRunning = true;
				_trackTimeToGenXPTaskRoutine.Start((Action<PausableTaskException>)null, (Action)null);
				_trackPlayerInactivityTaskRoutine.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private void StopRoutines()
		{
			if (_areRoutinesRunning)
			{
				_trackTimeToGenXPTaskRoutine.Stop();
				_trackPlayerInactivityTaskRoutine.Stop();
				_areRoutinesRunning = false;
			}
		}

		private IEnumerator HandleAnalytics()
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			TaskService<uint[]> xpRequest = serviceRequestFactory.Create<ILoadTotalXPRequest>().AsTask();
			yield return xpRequest;
			if (!xpRequest.succeeded)
			{
				Console.LogError("Load Total Xp Request failed. " + xpRequest.behaviour.exceptionThrown);
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			int updatedXP = Convert.ToInt32(xpRequest.result[0]);
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceRequestFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
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
			TaskService<TiersData> loadTiersBandingRequest = serviceRequestFactory.Create<ILoadTiersBandingRequest>().AsTask();
			yield return loadTiersBandingRequest;
			if (!loadTiersBandingRequest.succeeded)
			{
				Console.LogError("Load Tiers Banding Request failed. " + loadTiersBandingRequest.behaviour.exceptionThrown);
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			TiersData tiersData = loadTiersBandingRequest.result;
			uint cpu = garagePresenter.CurrentTotalRobotCPU;
			uint tier = RRAndTiers.ConvertRRToTierIndex(isMegabot: cpu > cpuPower.MaxCpuPower, totalRobotRanking: garagePresenter.CurrentTotalRobotRanking, tiersData: tiersData) + 1;
			LogPlayerXpEarnedDependency playerXpEarnedDependency = new LogPlayerXpEarnedDependency(_totalEarnedXP, updatedXP, roboPassXP, _startingPlayerLevel, 0, "EditMode", "T" + tier);
			TaskService logPlayerXpEarnedRequest = analyticsRequestFactory.Create<ILogPlayerXpEarnedRequest, LogPlayerXpEarnedDependency>(playerXpEarnedDependency).AsTask();
			yield return logPlayerXpEarnedRequest;
			if (!logPlayerXpEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Xp Request failed. " + logPlayerXpEarnedRequest.behaviour.exceptionThrown);
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
