using Robocraft.GUI;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class ContextSensitiveXPRefresher : IContextSensitiveXPRefresher, ITickable, ITickableBase
	{
		private const float TIME_BETWEEN_XP_REFRESH_SECONDS = 600f;

		private List<IDataSource> _dataSourcesToRefreshList = new List<IDataSource>();

		private float _timeConcurrentlyViewingScreen;

		private bool _canRefreshData;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		public void SetDataSources(List<IDataSource> dataSourcesDependantonXP)
		{
			_dataSourcesToRefreshList = new List<IDataSource>(dataSourcesDependantonXP);
		}

		public void ClanScreenShown()
		{
			_timeConcurrentlyViewingScreen = 0f;
			_canRefreshData = true;
		}

		public void ClanScreenHidden()
		{
			_timeConcurrentlyViewingScreen = 0f;
			_canRefreshData = false;
		}

		public void Tick(float deltaSec)
		{
			_timeConcurrentlyViewingScreen += deltaSec;
			if (_timeConcurrentlyViewingScreen > 600f && _canRefreshData)
			{
				_timeConcurrentlyViewingScreen = 0f;
				ExecuteXPRefresh();
			}
		}

		private void ExecuteXPRefresh()
		{
			IGetMyClanInfoRequest getMyClanInfoRequest = socialRequestFactory.Create<IGetMyClanInfoRequest>();
			getMyClanInfoRequest.SetAnswer(new ServiceAnswer<ClanInfo>(delegate(ClanInfo claninfo)
			{
				if (claninfo != null)
				{
					IPollClanExperienceRequest pollClanExperienceRequest = socialRequestFactory.Create<IPollClanExperienceRequest>();
					pollClanExperienceRequest.Inject(claninfo.ClanName);
					pollClanExperienceRequest.SetAnswer(new ServiceAnswer<PollClanExperienceRequestResponse>(delegate
					{
						RefreshRegisteredDataSources();
					}, delegate
					{
						Console.Log("Poll clan XP request failed.");
					})).Execute();
				}
			}, delegate
			{
				Console.Log("Failed fetching my clan info");
			})).Execute();
		}

		private void RefreshRegisteredDataSources()
		{
			foreach (IDataSource dataSourcesToRefresh in _dataSourcesToRefreshList)
			{
				dataSourcesToRefresh.RefreshData(delegate
				{
					Console.Log("Data source refreshed");
				}, delegate
				{
					Console.Log("Error refreshing data source from context sensitive xp refresher class");
				});
			}
		}
	}
}
