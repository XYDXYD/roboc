using Svelto.ES.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal interface IMapPingSpawnerComponent : IComponent
	{
		event Action OnPingDestroyed;

		event Action<float, float, int> InitializePingTimesAndNumber;

		void ShowPingAtLocation(Vector3 location, PingType type, string user, float life);
	}
}
