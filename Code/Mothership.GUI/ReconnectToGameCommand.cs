using LobbyServiceLayer;
using Services.Analytics;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI
{
	internal class ReconnectToGameCommand : IInjectableCommand<EnterBattleDependency>, ICommand
	{
		private EnterBattleDependency _dependency;

		[Inject]
		internal LobbyPresenter _lobbyPresenter
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
			AnalyticsCacheDTO.timeWhenAskedToReconnect = Time.get_realtimeSinceStartup();
			_lobbyPresenter.Reconnect(_dependency);
		}

		public ICommand Inject(EnterBattleDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logAskedToReconnectRequest = analyticsRequestFactory.Create<ILogAskedToReconnectRequest>().AsTask();
			yield return logAskedToReconnectRequest;
			if (!logAskedToReconnectRequest.succeeded)
			{
				throw new Exception("Log Asked To Reconnect Request failed", logAskedToReconnectRequest.behaviour.exceptionThrown);
			}
		}
	}
}
