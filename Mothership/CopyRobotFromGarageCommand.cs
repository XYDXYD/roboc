using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

namespace Mothership
{
	internal class CopyRobotFromGarageCommand : IInjectableCommand<int>, ICommand
	{
		private RobotSanctionData _robotSanction;

		private int _garageID;

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
		internal IServiceRequestFactory serviceFactory
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
		internal IGUIInputControllerMothership guiInputController
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
		internal RobotSanctionController robotSanctionController
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
			TaskRunner.get_Instance().Run(VerifyCopyRobot(_garageID));
		}

		public ICommand Inject(int garageID)
		{
			_garageID = garageID;
			return this;
		}

		private IEnumerator VerifyCopyRobot(int garageID)
		{
			loadingPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
			yield return robotSanctionController.CheckRobotSanction(garagePresenter.CurrentRobotIdentifier.ToString(), delegate(RobotSanctionData sanction)
			{
				TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
			});
			if (_robotSanction != null)
			{
				yield break;
			}
			ILoadGarageSlotLimitRequest garageSlotLimitRequest = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
			TaskService<uint> garageSlotLimitTask = new TaskService<uint>(garageSlotLimitRequest);
			yield return new HandleTaskServiceWithError(garageSlotLimitTask, delegate
			{
				loadingPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
			}).GetEnumerator();
			if (machineMap.GetNumberCubes() <= 0)
			{
				loadingPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strNoCubesInBay"), StringTableBase<StringTable>.Instance.GetString("strUnableToCopyEmpty")));
				yield break;
			}
			uint slotLimit = garageSlotLimitTask.result;
			if (garagePresenter.GarageSlotCount >= slotLimit)
			{
				loadingPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				ShowSlotLimitError(slotLimit);
				yield break;
			}
			ICopyReadOnlyRobotFromGarageRequest copyRequest = serviceFactory.Create<ICopyReadOnlyRobotFromGarageRequest, int>(garageID);
			TaskService copyTask = new TaskService(copyRequest);
			yield return copyTask;
			if (!copyTask.succeeded)
			{
				loadingPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				if (copyTask.behaviour.errorCode == 18)
				{
					ShowSlotLimitError(slotLimit);
				}
				if (copyTask.behaviour.errorCode == 145)
				{
					yield return robotSanctionController.CheckRobotSanction(garagePresenter.CurrentRobotIdentifier.ToString(), delegate(RobotSanctionData sanction)
					{
						TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
					});
				}
				else
				{
					ErrorWindow.ShowServiceErrorWindow(copyTask.behaviour);
				}
				yield break;
			}
			yield return garagePresenter.RefreshGarageData();
			garagePresenter.ShowGarageSlots();
			garagePresenter.LoadAndBuildRobot();
			if (copyTask.succeeded)
			{
				CreateNewRobotDependency createNewRobotDependency = new CreateNewRobotDependency(CreateNewRobotType.GARAGE_COPY);
				createdNewRobotObservable.Dispatch(ref createNewRobotDependency);
			}
			loadingPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
		}

		private IEnumerator OnRobotSanctionTaskSucceeded(RobotSanctionData robotSanction)
		{
			if (robotSanction != null)
			{
				_robotSanction = robotSanction;
				yield return garagePresenter.RefreshGarageData();
				garagePresenter.ShowGarageSlots();
				garagePresenter.LoadAndBuildRobot();
				garagePresenter.SelectCurrentGarageSlot();
			}
		}

		private void ShowSlotLimitError(uint slotLimit)
		{
			transactionController.ShowTooManySlotsDialog(StringTableBase<StringTable>.Instance.GetReplaceString("strUnableToCopyMax", "{MAX_SLOTS}", slotLimit.ToString()));
		}
	}
}
