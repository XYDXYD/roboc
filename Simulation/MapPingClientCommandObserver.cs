using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class MapPingClientCommandObserver
	{
		public event Action<Vector3, PingType, int> ShowPing = delegate
		{
		};

		public void SpawnPing(Vector3 location, PingType type, int senderId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.ShowPing(location, type, senderId);
		}
	}
}
