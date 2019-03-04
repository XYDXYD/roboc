using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class CrfItemListLoader
	{
		private Action<List<CRFItem>> _onComplete;

		private Action<Exception> _onFail;

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
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

		public bool isLoading
		{
			get;
			private set;
		}

		public uint lastRequestId
		{
			get;
			private set;
		}

		public CrfItemListLoader()
		{
			isLoading = false;
		}

		public void LoadItemList(CRFShopItemListDependency parameters, Action<List<CRFItem>> onComplete, Action<Exception> onFail)
		{
			_onComplete = onComplete;
			_onFail = onFail;
			isLoading = true;
			uint nextRequestId = ++lastRequestId;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)(() => LoadItemList(parameters, nextRequestId)));
		}

		private IEnumerator LoadItemList(CRFShopItemListDependency parameters, uint requestId)
		{
			List<CRFItem> result = new List<CRFItem>();
			ILoadCRFItemListRequest request = serviceRequestFactory.Create<ILoadCRFItemListRequest, CRFShopItemListDependency>(parameters);
			request.Inject(parameters);
			TaskService<LoadCrfItemListRequestResponse> task = new TaskService<LoadCrfItemListRequestResponse>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			try
			{
				LoadCrfItemListRequestResponse result2 = task.result;
				result.AddRange(result2.robotShopItemList);
			}
			catch (Exception obj)
			{
				_onFail(obj);
			}
			isLoading = false;
			if (requestId == lastRequestId)
			{
				_onComplete(result);
			}
		}

		public void AbortLastLoad()
		{
			isLoading = false;
			lastRequestId++;
		}
	}
}
