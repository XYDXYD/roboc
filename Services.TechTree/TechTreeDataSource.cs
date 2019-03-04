using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using System.Collections.Generic;

namespace Services.TechTree
{
	internal class TechTreeDataSource : IDataSource<Dictionary<string, TechTreeItemData>>
	{
		[Inject]
		internal IServiceRequestFactory serviceFactory
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

		public IEnumerator GetDataAsync(Dictionary<string, TechTreeItemData> data)
		{
			IGetTechTreeDataRequest getTechTreeDataReq = serviceFactory.Create<IGetTechTreeDataRequest>();
			TaskService<Dictionary<string, TechTreeItemData>> getTechTreeDataTS = new TaskService<Dictionary<string, TechTreeItemData>>(getTechTreeDataReq);
			ShowLoadingScreen();
			yield return new HandleTaskServiceWithError(getTechTreeDataTS, ShowLoadingScreen, HideLoadingScreen).GetEnumerator();
			HideLoadingScreen();
			if (getTechTreeDataTS.succeeded)
			{
				Dictionary<string, TechTreeItemData>.Enumerator enumerator = getTechTreeDataTS.result.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, TechTreeItemData> current = enumerator.Current;
					string key = current.Key;
					TechTreeItemData value = current.Value;
					data.Add(key, value);
				}
			}
		}

		private void HideLoadingScreen()
		{
			loadingIconPresenter.NotifyLoadingDone("GarageRefreshLoadingIcon");
		}

		private void ShowLoadingScreen()
		{
			loadingIconPresenter.NotifyLoading("GarageRefreshLoadingIcon");
		}
	}
}
