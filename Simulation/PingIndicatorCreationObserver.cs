using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingIndicatorCreationObserver
	{
		public event Action<GameObject> OnPingIndicatorCreated = delegate
		{
		};

		public void PingIndicatorCreated(GameObject pingIndicator)
		{
			this.OnPingIndicatorCreated(pingIndicator);
		}
	}
}
