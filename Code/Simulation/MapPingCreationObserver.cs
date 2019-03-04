using System;
using UnityEngine;

namespace Simulation
{
	internal class MapPingCreationObserver
	{
		public event Action<GameObject, PingType, int> OnMapPingCreated = delegate
		{
		};

		public void MapPingCreated(GameObject ping, PingType type, int playerId)
		{
			this.OnMapPingCreated(ping, type, playerId);
		}
	}
}
