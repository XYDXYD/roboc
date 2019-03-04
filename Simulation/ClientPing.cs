using Network;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;

namespace Simulation
{
	internal sealed class ClientPing : ITickable, IWaitForFrameworkInitialization, ITickableBase
	{
		private const string PING_VIEW_PREFAB = "DebugPingView";

		private const float PING_UPDATE_RATE_SECONDS = 10f;

		private DateTime _lastUpdateTime;

		private int _totalPing;

		private int _totalRecords;

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		public event Action<string, int, int> OnPingUpdated;

		public ClientPing()
		{
			_lastUpdateTime = DateTime.UtcNow;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			gameObjectFactory.Build("DebugPingView");
		}

		public void Tick(float deltaTime)
		{
			if ((DateTime.UtcNow - _lastUpdateTime).TotalSeconds > 10.0)
			{
				GetPing();
				_lastUpdateTime = DateTime.UtcNow;
			}
		}

		private void GetPing()
		{
			if (NetworkClient.active)
			{
				NetworkClient networkClient = NetworkClient.allClients[0];
				int rTT = networkClient.GetRTT();
				if (this.OnPingUpdated != null)
				{
					this.OnPingUpdated(networkClient.serverIp, networkClient.serverPort, rTT);
				}
				_totalRecords++;
				_totalPing += rTT;
			}
		}
	}
}
