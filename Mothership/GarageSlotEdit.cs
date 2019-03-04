using Services.Analytics;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal sealed class GarageSlotEdit
	{
		private GaragePresenter garage;

		private IServiceRequestFactory serviceFactory;

		private LoadingIconPresenter loadingIconPresenter;

		private IAnalyticsRequestFactory analyticsRequestFactory;

		private GarageView _garageView;

		public GarageSlotEdit(GaragePresenter pGarage, IServiceRequestFactory pServiceFactory, LoadingIconPresenter pLoadingIconPresenter, IAnalyticsRequestFactory pAnalyticsRequestFactory)
		{
			garage = pGarage;
			serviceFactory = pServiceFactory;
			loadingIconPresenter = pLoadingIconPresenter;
			analyticsRequestFactory = pAnalyticsRequestFactory;
		}

		public void SwitchSlot(uint slotId)
		{
			if (slotId != garage.currentGarageSlot)
			{
				garage.SetGarageSlot(slotId);
				garage.UpdateCurrentName();
				garage.LoadAndBuildRobot();
				garage.SelectCurrentGarageSlot();
				TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleGarageSlotSelectedAnalytics);
			}
		}

		public void RefreshSlot(uint slotId)
		{
			IClearMachineRequest clearMachineRequest = serviceFactory.Create<IClearMachineRequest, uint>(slotId);
			clearMachineRequest.Execute();
		}

		public void SetCurrentSlotName(string newName)
		{
			if (garage.SetCurrentName(newName))
			{
				loadingIconPresenter.NotifyLoading("GarageSlotLoadingIcon");
				ISetRobotNameRequest setRobotNameRequest = serviceFactory.Create<ISetRobotNameRequest, SetRobotNameDependency>(new SetRobotNameDependency(garage.currentGarageSlot, newName));
				setRobotNameRequest.SetAnswer(new ServiceAnswer<string>(OnSetNameSucceeded, OnGarageOperationFailed));
				setRobotNameRequest.Execute();
			}
		}

		private void OnSetNameSucceeded(string response)
		{
			loadingIconPresenter.NotifyLoadingDone("GarageSlotLoadingIcon");
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleRobotNameChangedAnalytics);
		}

		private void OnGarageOperationFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour, delegate
			{
				loadingIconPresenter.NotifyLoading("GarageSlotLoadingIcon");
			});
			loadingIconPresenter.NotifyLoadingDone("GarageSlotLoadingIcon");
		}

		private IEnumerator HandleGarageSlotSelectedAnalytics()
		{
			TaskService logGarageSlotSelectedRequest = analyticsRequestFactory.Create<ILogGarageSlotSelectedRequest>().AsTask();
			yield return logGarageSlotSelectedRequest;
			if (!logGarageSlotSelectedRequest.succeeded)
			{
				throw new Exception("Log Garage Slot Selected Request failed", logGarageSlotSelectedRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator HandleRobotNameChangedAnalytics()
		{
			TaskService logRobotNameChangedRequest = analyticsRequestFactory.Create<ILogRobotNameChangedRequest>().AsTask();
			yield return logRobotNameChangedRequest;
			if (!logRobotNameChangedRequest.succeeded)
			{
				throw new Exception("Log Robot Name Changed Request failed", logRobotNameChangedRequest.behaviour.exceptionThrown);
			}
		}
	}
}
