using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Services.Analytics
{
	internal static class PurchaseFunnelHelper
	{
		private static string _startEventId;

		public static void SendEvent(IAnalyticsRequestFactory factory, string step, string context, bool startsNewChain)
		{
			if (startsNewChain)
			{
				_startEventId = Guid.NewGuid().ToString();
			}
			LogPurchaseFunnelDependency param = new LogPurchaseFunnelDependency(step, context, startsNewChain, _startEventId);
			TaskService task = factory.Create<ILogPurchaseFunnelRequest, LogPurchaseFunnelDependency>(param).AsTask();
			TaskRunner.get_Instance().Run(HandleRequest(task));
		}

		private static IEnumerator HandleRequest(TaskService task)
		{
			yield return task;
			if (!task.succeeded)
			{
				string text = "Log Purchase Funnel failed. " + task.behaviour.exceptionThrown;
				Console.LogError(text);
			}
		}
	}
}
