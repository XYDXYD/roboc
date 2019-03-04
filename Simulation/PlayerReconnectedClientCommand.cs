using Services.Analytics;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class PlayerReconnectedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIdDependency _dependency;

		[Inject]
		internal VOManager _voManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer _teams
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

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (PlayerIdDependency)dependency;
			return this;
		}

		public void Execute()
		{
			if (_teams.IsMe(TargetType.Player, _dependency.owner))
			{
				float num = Time.get_realtimeSinceStartup() - AnalyticsCacheDTO.timeWhenAskedToReconnect;
				TaskRunner.get_Instance().Run(HandleAnalytics(num));
				Console.Log("Took " + num + "s to reconnect");
				_voManager.PlayVO(AudioFabricEvent.StringEvents[173]);
			}
			else
			{
				_voManager.PlayVO(AudioFabricEvent.StringEvents[172]);
			}
		}

		private IEnumerator HandleAnalytics(float timeTaken)
		{
			TaskService logReconnectedRequest = analyticsRequestFactory.Create<ILogReconnectedRequest, float>(timeTaken).AsTask();
			yield return logReconnectedRequest;
			if (!logReconnectedRequest.succeeded)
			{
				throw new Exception("Log Reconnected Request failed", logReconnectedRequest.behaviour.exceptionThrown);
			}
		}
	}
}
