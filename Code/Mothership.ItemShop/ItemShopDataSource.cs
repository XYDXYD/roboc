using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;

namespace Mothership.ItemShop
{
	internal sealed class ItemShopDataSource
	{
		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		public IEnumerator LoadData(bool clearCache = false)
		{
			ItemShopResponseData response = null;
			loadingIconPresenter.NotifyLoading("LoadingItemShop");
			ILoadItemShopBundleListRequest request = serviceFactory.Create<ILoadItemShopBundleListRequest>();
			TaskService<ItemShopResponseData> task = new TaskService<ItemShopResponseData>(request);
			if (clearCache)
			{
				request.ClearCache();
			}
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingItemShop");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingItemShop");
			}).GetEnumerator();
			if (task.succeeded)
			{
				response = task.result;
			}
			loadingIconPresenter.NotifyLoadingDone("LoadingItemShop");
			yield return response;
		}
	}
}
