using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class RealMoneyStorePossibleRoboPassItemsDataSource : IRealMoneyStorePossibleRoboPassItemsDataSource
	{
		private List<RoboPassPreviewItemDisplayData> _loadedPreviewRoboPassItems = new List<RoboPassPreviewItemDisplayData>();

		private readonly IServiceRequestFactory _webservicesRequestFactory;

		private readonly LoadingIconPresenter _loadingIcon;

		public event Action<bool> OnDataChanged;

		public RealMoneyStorePossibleRoboPassItemsDataSource(LoadingIconPresenter loadingIcon, IServiceRequestFactory serviceRequestFactory)
		{
			_webservicesRequestFactory = serviceRequestFactory;
			_loadingIcon = loadingIcon;
		}

		public int GetDataItemsCount()
		{
			return _loadedPreviewRoboPassItems.Count;
		}

		public RoboPassPreviewItemDisplayData GetDataItem(int index)
		{
			return _loadedPreviewRoboPassItems[index];
		}

		public IEnumerator LoadData()
		{
			_loadingIcon.NotifyLoading("RealMoneyStorePossibleRoboPassItems");
			IGetRoboPassPreviewItemsRequest getRoboPassPreviewItemsRequest = _webservicesRequestFactory.Create<IGetRoboPassPreviewItemsRequest>();
			getRoboPassPreviewItemsRequest.ClearCache();
			TaskService<IList<RoboPassPreviewItemDisplayData>> getItemsTask = new TaskService<IList<RoboPassPreviewItemDisplayData>>(getRoboPassPreviewItemsRequest);
			yield return getItemsTask;
			_loadingIcon.NotifyLoadingDone("RealMoneyStorePossibleRoboPassItems");
			if (getItemsTask.succeeded)
			{
				IList<RoboPassPreviewItemDisplayData> result = getItemsTask.result;
				foreach (RoboPassPreviewItemDisplayData item in result)
				{
					_loadedPreviewRoboPassItems.Add(item);
				}
				SafeEvent.SafeRaise<bool>(this.OnDataChanged, true);
			}
			else
			{
				Console.LogWarning("Warning - failed to retrieve preview items request.");
			}
		}
	}
}
