using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class PingView : MonoBehaviour, IInitialize
	{
		public UILabel _label;

		[Inject]
		internal ClientPing clientPing
		{
			private get;
			set;
		}

		public PingView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			clientPing.OnPingUpdated += OnPingUpdated;
		}

		private void OnDestroy()
		{
			if (clientPing != null)
			{
				clientPing.OnPingUpdated -= OnPingUpdated;
			}
		}

		private void OnPingUpdated(string ip, int port, int ping)
		{
			_label.set_text("Ping To GameServer [" + ip + ":" + port + "]: " + ping.ToString());
		}

		private void Update()
		{
			if (Input.GetKey(308) && Input.GetKeyUp(46))
			{
				_label.set_enabled(!_label.get_enabled());
			}
		}
	}
}
