using Services.Analytics;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership.Analytics
{
	internal class WorldSwitchingMothershipAnalytics : WorldSwitchingAnalytics, IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal WorldSwitching worldSwitch
		{
			private get;
			set;
		}

		[Inject]
		internal ITutorialController tutorialController
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
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IAutoSaveController autoSaveController
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
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			worldSwitch.OnWorldJustSwitched += OnWorldSwitched;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			worldSwitch.OnWorldJustSwitched -= OnWorldSwitched;
		}

		private IEnumerator OnWorldSwitching()
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			LogPlayerLeftMothershipDependency playerLeftMothershipDependency = new LogPlayerLeftMothershipDependency(base.currentLevelName, base.currentGameModeType, Convert.ToInt32(Time.get_time() - worldSwitch.CurrentWorldStartTime));
			TaskService logPlayerLeftMothershipRequest = analyticsRequestFactory.Create<ILogPlayerLeftMothershipRequest, LogPlayerLeftMothershipDependency>(playerLeftMothershipDependency).AsTask();
			yield return logPlayerLeftMothershipRequest;
			if (!logPlayerLeftMothershipRequest.succeeded)
			{
				Console.LogError("Log Player Left Mothership Request failed. " + logPlayerLeftMothershipRequest.behaviour.exceptionThrown);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private void OnWorldSwitched(WorldSwitchMode currentMode)
		{
			worldSwitch.OnWorldIsSwitching.Add(OnWorldSwitching());
			string levelName = currentMode.ToString();
			string gameModeTypeForAnalytics = GetGameModeTypeForAnalytics(currentMode);
			SetCurrentWorld(levelName, gameModeTypeForAnalytics);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SendLevelStarted);
		}

		private IEnumerator SendLevelStarted()
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			ILoadPlayerDataRequest request = serviceFactory.Create<ILoadPlayerDataRequest>();
			TaskService<PlayerDataResponse> task = new TaskService<PlayerDataResponse>(request);
			yield return task;
			LogPlayerEnteredMothershipDependency playerEnteredMothershipDependency;
			if (task.succeeded)
			{
				PlayerDataResponse result = task.result;
				uint robotCPU = result.robotCPU;
				bool isCRFBot_ = result.crfId != 0;
				string controlType_ = result.controlSetting.controlType.ToString();
				bool verticalStrafing = result.controlSetting.verticalStrafing;
				int cubeCategoryCount = GetCubeCategoryCount(machineMap.GetAllInstantiatedCubes().GetEnumerator(), CubeCategory.Cosmetic);
				playerEnteredMothershipDependency = new LogPlayerEnteredMothershipDependency(base.currentLevelName, base.currentGameModeType, robotCPU, isCRFBot_, controlType_, verticalStrafing, cubeCategoryCount);
			}
			else
			{
				playerEnteredMothershipDependency = new LogPlayerEnteredMothershipDependency(base.currentLevelName, base.currentGameModeType);
			}
			TaskService logPlayerEnteredMothershipRequest = analyticsRequestFactory.Create<ILogPlayerEnteredMothershipRequest, LogPlayerEnteredMothershipDependency>(playerEnteredMothershipDependency).AsTask();
			yield return logPlayerEnteredMothershipRequest;
			if (!logPlayerEnteredMothershipRequest.succeeded)
			{
				Console.LogError("Log Player Entered Mothership Request failed. " + logPlayerEnteredMothershipRequest.behaviour.exceptionThrown);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private string GetGameModeTypeForAnalytics(WorldSwitchMode currentMode)
		{
			string result = GetGameModeType();
			if (currentMode == WorldSwitchMode.BuildMode || currentMode == WorldSwitchMode.GarageMode)
			{
				if (tutorialController.IsActive() || WorldSwitching.IsTutorial())
				{
					result = "Tutorial";
				}
				else
				{
					switch (currentMode)
					{
					case WorldSwitchMode.BuildMode:
						result = "BuildMode";
						break;
					case WorldSwitchMode.GarageMode:
						result = "GarageMode";
						break;
					default:
						result = string.Empty;
						break;
					}
				}
			}
			return result;
		}
	}
}
