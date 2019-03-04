using Services.Analytics;
using Services.Requests.Interfaces;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Utility;
using WebServices;

namespace Login
{
	internal sealed class InitialiseAndLoadLevel
	{
		public static string newSceneName = "RC_Mothership";

		public static SerialTaskCollection LoadUserData(Action<ServiceBehaviour> failQueue, bool justRegisteredUser, GenericLoadingScreen loadingScreen)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Expected O, but got Unknown
			IServiceRequestFactory serviceRequestFactory = new WebStorageRequestFactoryDefault();
			ISocialRequestFactory socialRequestFactory = new SocialRequestFactory();
			IAnalyticsRequestFactory analyticsRequestFactory = new AnalyticsRequestFactory();
			SerialTaskCollection val = new SerialTaskCollection();
			if (loadingScreen != null)
			{
				loadingScreen.text = StringTableBase<StringTable>.Instance.GetString("strLoadingGameData");
			}
			ParallelTaskCollection val2 = new ParallelTaskCollection();
			val2.Add(ExecuteAsTask<ILoadCubeListRequest, ReadOnlyDictionary<CubeTypeID, CubeListData>>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadSpecialItemListRequest, ReadOnlyDictionary<uint, SpecialItemListData>>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadIncomeScalesPremiumFactorRequest, IncomeScalesResponse>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadDefaultColorPaletteRequest, ColorPaletteData>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<IGetGameClientSettingsRequest, GameClientSettingsDependency>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadRobotShopConfigRequest, LoadRobotShopConfigResponse>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadWeaponStatsRequest, IDictionary<int, WeaponStatsData>>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadMovementStatsRequest, MovementStats>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<IGetFriendListRequest, GetFriendListResponse>(socialRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<IGetPowerBarSettingsRequest, PowerBarSettingsData>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<IGetDamageBoostRequest, DamageBoostDeserialisedData>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadBattleArenaSettingsRequest, BattleArenaSettingsDependency>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadCpuSettingsRequest, CPUSettingsDependency>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadCosmeticsRenderLimitsRequest, CosmeticsRenderLimitsDependency>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<IGetTauntsRequest, TauntsDeserialisedData>(serviceRequestFactory, failQueue));
			val2.Add(ExecuteAsTask<ILoadAllCustomisationInfoRequest, AllCustomisationsResponse>(serviceRequestFactory, failQueue));
			TaskService<TiersData> taskService = new TaskService<TiersData>(serviceRequestFactory.Create<ILoadTiersBandingRequest>());
			val2.Add(taskService);
			val2.Add(socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>());
			val.Add((IEnumerator)val2);
			val2.add_onComplete((Action)delegate
			{
				UpdateBar(loadingScreen, 0.33f);
				ILogLoadingRequest logLoadingRequest2 = analyticsRequestFactory.Create<ILogLoadingRequest, string>("LoadUserDataTasks0End");
				logLoadingRequest2.Execute();
				if (loadingScreen != null)
				{
					loadingScreen.text = StringTableBase<StringTable>.Instance.GetString("strLoadingPlayerData");
				}
			});
			ParallelTaskCollection val3 = new ParallelTaskCollection();
			val3.Add(ExecuteAsTask<ILoadUserCubeInventoryRequest, Dictionary<uint, uint>>(serviceRequestFactory, failQueue));
			val3.Add(ExecuteAsTask<ILoadPlayerLevelDataRequest, IDictionary<uint, uint>>(serviceRequestFactory, failQueue));
			val3.Add(ExecuteAsTask<ILoadWalletRequest, Wallet>(serviceRequestFactory, failQueue));
			val3.Add(ExecuteAsTask<ILoadPremiumDataRequest, PremiumInfoData>(serviceRequestFactory, failQueue));
			ILoadTechPointsRequest loadTechPointsRequest = serviceRequestFactory.Create<ILoadTechPointsRequest>();
			loadTechPointsRequest.SetAnswer(new ServiceAnswer<int>(null, failQueue));
			TaskService<int> taskService2 = new TaskService<int>(loadTechPointsRequest);
			val3.Add(taskService2);
			val.Add((IEnumerator)val3);
			val3.add_onComplete((Action)delegate
			{
				UpdateBar(loadingScreen, 0.66f);
				ILogLoadingRequest logLoadingRequest = analyticsRequestFactory.Create<ILogLoadingRequest, string>("LoadUserDataTasks1End");
				logLoadingRequest.Execute();
				if (loadingScreen != null)
				{
					loadingScreen.text = StringTableBase<StringTable>.Instance.GetString("strLoadingAssets");
				}
			});
			return val;
		}

		private static IEnumerator ExecuteAsTask<TServiceRequest, TReturnData>(IServiceRequestFactory requestFactory, Action<ServiceBehaviour> onFail) where TServiceRequest : class, IServiceRequest, IAnswerOnComplete<TReturnData>
		{
			TaskService<TReturnData> requestTask = requestFactory.Create<TServiceRequest>().AsTask();
			yield return requestTask;
			if (!requestTask.succeeded)
			{
				onFail?.Invoke(requestTask.behaviour);
			}
		}

		public static ITaskRoutine StartLoadGame(bool justRegisteredUser, GenericLoadingScreen loadingScreen)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			loadingScreen.get_gameObject().SetActive(true);
			Console.Log("Start Game Loading");
			try
			{
				ITaskRoutine val = null;
				Action<ServiceBehaviour> failQueue = delegate(ServiceBehaviour sb)
				{
					Console.LogError("Initialization phase failure");
					throw new Exception(sb.errorBody);
				};
				if (loadingScreen.slider != null)
				{
					loadingScreen.slider.get_gameObject().SetActive(true);
				}
				ParallelTaskCollection val2 = new ParallelTaskCollection();
				ParallelTaskCollection val3 = new ParallelTaskCollection();
				SerialTaskCollection val4 = new SerialTaskCollection();
				SerialTaskCollection val5 = LoadUserData(failQueue, justRegisteredUser, loadingScreen);
				UpdateBar(loadingScreen, 0f);
				val5.add_onComplete((Action)delegate
				{
					UpdateBar(loadingScreen, 1f);
				});
				val3.Add(LogLoadGameTaskRoutineStart());
				val3.Add((IEnumerator)val5);
				val4.Add(LoadPlatformConfig());
				val4.Add((IEnumerator)val3);
				val4.Add(LoadStubLevel(loadingScreen));
				val2.Add((IEnumerator)val4);
				val = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)val2);
				val.Start((Action<PausableTaskException>)null, (Action)null);
				return val;
			}
			catch (Exception innerException)
			{
				throw new Exception("Exception while loading assets", innerException);
			}
		}

		private static IEnumerator LoadPlatformConfig()
		{
			IServiceRequestFactory serviceFactory = new WebStorageRequestFactoryDefault();
			ILoadPlatformConfigurationRequest loadPlatformConfigurationReq = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			yield return loadPlatformConfigurationReq.AsTask();
		}

		private static IEnumerator LogLoadGameTaskRoutineStart()
		{
			IAnalyticsRequestFactory analyticsRequestFactory = new AnalyticsRequestFactory();
			yield return analyticsRequestFactory.Create<ILogLoadingRequest, string>("LoadGameTaskRoutineStart").AsTask();
		}

		private static void UpdateBar(GenericLoadingScreen loadingScreen, float progress)
		{
			if (loadingScreen.slider != null)
			{
				loadingScreen.slider.set_value(progress);
				loadingScreen.slider.ForceUpdate();
			}
		}

		private static IEnumerator LoadStubLevel(GenericLoadingScreen loadingScreen)
		{
			loadingScreen.text = StringTableBase<StringTable>.Instance.GetString("strInitialising");
			yield return SceneManager.LoadSceneAsync(newSceneName);
		}
	}
}
