using Services.Analytics;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class CopyRobotFromCRFCommand : IInjectableCommand<CRFItem>, ICommand
	{
		private CRFItem _item;

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
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal RobotCostCalculator robotCostCalculator
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopTransactionController transactionController
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
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
		internal RobotShopObserver observer
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
		internal WeaponOrderManager weaponOrderManager
		{
			private get;
			set;
		}

		[Inject]
		private CreatedNewRobotObservable_Tencent createdNewRobotObservable
		{
			get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run(VerifyCopyRobot(_item));
		}

		public ICommand Inject(CRFItem item)
		{
			_item = item;
			return this;
		}

		private IEnumerator VerifyCopyRobot(CRFItem item)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			if ((item.robotShopItem.submissionExpiryDate - DateTime.UtcNow).TotalSeconds <= 0.0)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarningTitle"), StringTableBase<StringTable>.Instance.GetString("strUnableToGetExpired")));
				yield break;
			}
			ILoadGarageSlotLimitRequest request = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
			TaskService<uint> task = new TaskService<uint>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			if (!task.succeeded)
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
				yield break;
			}
			uint slotLimit = task.result;
			if (garagePresenter.GarageSlotCount >= slotLimit)
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
				ShowSlotLimitError(slotLimit);
				yield break;
			}
			List<uint> lockedCubes = item.LockedCubes;
			if (lockedCubes.Count > 0)
			{
				Dictionary<CubeTypeID, SunkCube> robotCubesInfoFromIDs = robotCostCalculator.GetRobotCubesInfoFromIDs(lockedCubes);
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
				transactionController.ShowRobotLockedDialog(StringTableBase<StringTable>.Instance.GetString("strTechTreeCRFTitle"), StringTableBase<StringTable>.Instance.GetString("strTechTreeCRFBody"), robotCubesInfoFromIDs);
				yield break;
			}
			IAnswerOnComplete copyRequest = serviceFactory.Create<ICopyAndConstructRobotFromCRFRequest, CopyAndConstructRobotDependency>(new CopyAndConstructRobotDependency(item.robotShopItem.id, 0u, 0u));
			TaskService copyTask = new TaskService(copyRequest);
			yield return copyTask;
			if (!copyTask.succeeded)
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
				short errorCode = copyTask.behaviour.errorCode;
				if (copyTask.behaviour.errorCode == 18)
				{
					ShowSlotLimitError(slotLimit);
				}
				else if (errorCode == 140 || errorCode == 144)
				{
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarningTitle"), StringTableBase<StringTable>.Instance.GetString("strUnableToUnlockExpired"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
					{
						observer.OnHideRobot(refreshList: true);
					}));
				}
				else
				{
					ErrorWindow.ShowServiceErrorWindow(copyTask.behaviour);
				}
				yield break;
			}
			yield return garagePresenter.RefreshGarageData();
			garagePresenter.ShowGarageSlots();
			guiInputController.ShowScreen(GuiScreens.Garage);
			if (copyTask.succeeded)
			{
				CreateNewRobotDependency createNewRobotDependency = new CreateNewRobotDependency(CreateNewRobotType.FROM_CRF, _item.robotShopItem.cubeData);
				createdNewRobotObservable.Dispatch(ref createNewRobotDependency);
			}
			yield return HandleAnalytics();
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
		}

		private void ShowSlotLimitError(uint slotLimit)
		{
			transactionController.ShowTooManySlotsDialog(StringTableBase<StringTable>.Instance.GetReplaceString("strCRFMaxSlots", "{MAX_SLOTS}", slotLimit.ToString()));
		}

		private IEnumerator HandleAnalytics()
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			TaskService<LoadGarageDataRequestResponse> loadGarageDataRequest = serviceFactory.Create<ILoadGarageDataRequest>().AsTask();
			yield return loadGarageDataRequest;
			if (!loadGarageDataRequest.succeeded)
			{
				Console.LogError("Log RobotShop Downloaded request failed while retrieving Garage Data. " + loadGarageDataRequest.behaviour.exceptionThrown);
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			TaskService<TiersData> loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>().AsTask();
			yield return loadTiersBandingReq;
			if (loadTiersBandingReq.succeeded)
			{
				TiersData tiersData = loadTiersBandingReq.result;
				uint cpu = garagePresenter.CurrentTotalRobotCPU;
				uint tier = RRAndTiers.ConvertRRToTierIndex(isMegabot: cpu > cpuPower.MaxCpuPower, totalRobotRanking: garagePresenter.CurrentTotalRobotRanking, tiersData: tiersData) + 1;
				LogRobotDownloadedDependency robotShopDownloadedDependency = new LogRobotDownloadedDependency(tier, cpu, garagePresenter.CurrentMovementCategories, weaponOrderManager.weaponOrder.GetItemCategories());
				TaskService logRobotShopDownloadedRequest = analyticsRequestFactory.Create<ILogRobotDownloadedRequest, LogRobotDownloadedDependency>(robotShopDownloadedDependency).AsTask();
				yield return logRobotShopDownloadedRequest;
				if (!logRobotShopDownloadedRequest.succeeded)
				{
					Console.LogError("Log RobotShop Downloaded Request failed. " + logRobotShopDownloadedRequest.behaviour.exceptionThrown);
					loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				}
				else
				{
					loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				}
			}
			else
			{
				Console.LogError("Log RobotShop Downloaded request failed while retrieving Tiers Banding. " + loadTiersBandingReq.behaviour.exceptionThrown);
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
			}
		}
	}
}
