using RCNetwork.Events;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation
{
	internal class MultiplayerBattleLongPlayMultiplier_Tencent : IInitialize, IWaitForFrameworkDestruction
	{
		private const float LONG_PLAY_POLL_INITIAL_WAIT = 10f;

		private const float LONG_PLAY_POLL_TIME_SECONDS = 120f;

		private ITaskRoutine _pollLongPlayValue;

		private LongPlayMultiplierDataDependancy_Tencent _dataDependancy = new LongPlayMultiplierDataDependancy_Tencent();

		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_pollLongPlayValue = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)PollLongPlayValue);
			_pollLongPlayValue.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private IEnumerator PollLongPlayValue()
		{
			yield return (object)new WaitForSecondsEnumerator(10f);
			SendNewLongPlayValueToServer();
			while (true)
			{
				yield return (object)new WaitForSecondsEnumerator(120f);
				SendNewLongPlayValueToServer();
			}
		}

		private void SendNewLongPlayValueToServer()
		{
			IGetLongPlayMultiplierRequest getLongPlayMultiplierRequest = serviceRequestFactory.Create<IGetLongPlayMultiplierRequest>();
			getLongPlayMultiplierRequest.SetAnswer(new ServiceAnswer<float>(delegate(float longPlayMultiplier)
			{
				_dataDependancy.LongPlayClientMultiplier = longPlayMultiplier;
				eventManagerClient.SendEventToServer(NetworkEvent.longPlayValue, _dataDependancy);
			}));
			getLongPlayMultiplierRequest.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			if (_pollLongPlayValue != null)
			{
				_pollLongPlayValue.Stop();
			}
		}
	}
}
