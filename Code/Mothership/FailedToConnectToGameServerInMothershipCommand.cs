using Services.Analytics;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class FailedToConnectToGameServerInMothershipCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
			lobbyPresenter.FailedToConnectToGameServer();
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logErrorRequest = analyticsRequestFactory.Create<ILogErrorRequest, string>("FailedToConnectToGameServer").AsTask();
			yield return logErrorRequest;
			if (!logErrorRequest.succeeded)
			{
				throw new Exception("Log Error Request failed", logErrorRequest.behaviour.exceptionThrown);
			}
		}
	}
}
