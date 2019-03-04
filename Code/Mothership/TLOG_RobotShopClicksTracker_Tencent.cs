using Services.Analytics;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class TLOG_RobotShopClicksTracker_Tencent : IInitialize, IWaitForFrameworkDestruction
	{
		private int _clickCount = -1;

		[Inject]
		private RobotShopObserver robotShopObserver
		{
			get;
			set;
		}

		[Inject]
		private IAnalyticsRequestFactory analyticsRequestFactory
		{
			get;
			set;
		}

		public void OnDependenciesInjected()
		{
			robotShopObserver.OnRobotShopOpenedEvent += LogRobotShopOpened;
			robotShopObserver.OnShowMoreRobotsEvent += LogPlayerClickedOnShowMore;
		}

		public void OnFrameworkDestroyed()
		{
			robotShopObserver.OnRobotShopOpenedEvent -= LogRobotShopOpened;
			robotShopObserver.OnShowMoreRobotsEvent -= LogPlayerClickedOnShowMore;
		}

		private void LogRobotShopOpened()
		{
			_clickCount = 0;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LogPlayerClickedRobotShop);
		}

		private void LogPlayerClickedOnShowMore()
		{
			_clickCount++;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LogPlayerClickedRobotShop);
		}

		private IEnumerator LogPlayerClickedRobotShop()
		{
			ILogPlayerClickedInRobotShopRequest service = analyticsRequestFactory.Create<ILogPlayerClickedInRobotShopRequest, int>(_clickCount);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Clicked Robot Shop Log Request failed to send " + task.behaviour.errorBody);
			}
		}
	}
}
