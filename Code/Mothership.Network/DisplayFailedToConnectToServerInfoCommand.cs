using Battle;
using Services.Analytics;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.Network
{
	internal sealed class DisplayFailedToConnectToServerInfoCommand : IInjectableCommand<string>, ICommand
	{
		private string _dependency;

		private float _pingTimeout = 5f;

		[Inject]
		public IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		public IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public ICommand Inject(string dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run(PingServerIp(_pingTimeout));
		}

		private void ShowErrorWindow(bool ipUnreachable)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
			string @string;
			string string2;
			if (ipUnreachable)
			{
				RemoteLogger.Error("failed to connect to game server, ip unreachable", battleParameters.HostIP + ":" + battleParameters.HostPort + "(UnityError:" + _dependency + ") Client failed to connect to gameserver", null);
				@string = StringTableBase<StringTable>.Instance.GetString("strNetworkError");
				string2 = StringTableBase<StringTable>.Instance.GetString("strFailConnectGameIP");
			}
			else
			{
				RemoteLogger.Error("failed to connect to game server, port unreachable: ", battleParameters.HostIP + ":" + battleParameters.HostPort + "(UnityError:" + _dependency + ") Client failed to connect to gameserver", null);
				@string = StringTableBase<StringTable>.Instance.GetString("strNetworkError");
				string2 = StringTableBase<StringTable>.Instance.GetString("strFailConnectGameServerPortAndIP");
			}
			string2 = string2.Replace("[IP]", battleParameters.HostIP);
			string2 = string2.Replace("[PORT]", battleParameters.HostPort.ToString());
			string2 = string2.Replace("[ERRORSTRING]", _dependency);
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			GenericErrorData error = new GenericErrorData(@string, string2, StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
			});
			ErrorWindow.ShowErrorWindow(error);
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

		private IEnumerator PingServerIp(float timeout)
		{
			Ping ping = new Ping(battleParameters.HostIP);
			float startTime = Time.get_time();
			bool timedout = false;
			while (!ping.get_isDone())
			{
				yield return null;
				if (Time.get_time() - startTime > timeout)
				{
					timedout = true;
					break;
				}
			}
			ShowErrorWindow(timedout);
		}
	}
}
