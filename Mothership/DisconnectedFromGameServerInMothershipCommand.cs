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
	internal class DisconnectedFromGameServerInMothershipCommand : IDispatchableCommand, ICommand
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
			lobbyPresenter.DisconnectedFromGameServer();
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logErrorRequest = analyticsRequestFactory.Create<ILogErrorRequest, string>("DisconnectionBeforeBattle").AsTask();
			yield return logErrorRequest;
			if (!logErrorRequest.succeeded)
			{
				throw new Exception("Log Error Request failed", logErrorRequest.behaviour.exceptionThrown);
			}
		}
	}
}
