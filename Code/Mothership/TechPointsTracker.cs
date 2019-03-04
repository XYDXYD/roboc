using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal sealed class TechPointsTracker : IWaitForFrameworkInitialization
	{
		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		internal event Action<int> OnUserTechPointsAmountChanged;

		public TechPointsTracker()
		{
			this.OnUserTechPointsAmountChanged = delegate
			{
			};
		}

		public void OnFrameworkInitialized()
		{
			RefreshUserTechPointsAmount();
		}

		internal void RefreshUserTechPointsAmount(Action<int> callback = null)
		{
			TaskRunner.get_Instance().Run(RefreshUserTechPointsAmountEnumerator(callback));
		}

		internal IEnumerator RefreshUserTechPointsAmountEnumerator(Action<int> callback = null)
		{
			ILoadTechPointsRequest loadTechPointsReq = serviceRequestFactory.Create<ILoadTechPointsRequest>();
			loadTechPointsReq.ClearCache();
			TaskService<int> loadTechPointsTS = new TaskService<int>(loadTechPointsReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadTechPointsTS, delegate
			{
				loadingIconPresenter.NotifyLoading("TechPointsTrackerService");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("TechPointsTrackerService");
			});
			yield return handleTSWithError.GetEnumerator();
			if (loadTechPointsTS.succeeded)
			{
				loadingIconPresenter.NotifyLoading("TechPointsTrackerService");
				Console.Log("User tech points amount refreshed");
				int userTechPointsAmount = loadTechPointsTS.result;
				if (this.OnUserTechPointsAmountChanged != null)
				{
					this.OnUserTechPointsAmountChanged(userTechPointsAmount);
				}
				callback?.Invoke(userTechPointsAmount);
				loadingIconPresenter.NotifyLoadingDone("TechPointsTrackerService");
			}
		}
	}
}
