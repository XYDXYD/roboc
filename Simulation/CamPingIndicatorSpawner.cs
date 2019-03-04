using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class CamPingIndicatorSpawner : MonoBehaviour
	{
		[SerializeField]
		private GameObject camPingIndicator;

		[SerializeField]
		private float radius = 1f;

		public int queueSize = 3;

		private CamPingIndicatorBehaviour[] _camPings;

		private int _lastFreePosition;

		[Inject]
		internal IGameObjectFactory factory
		{
			private get;
			set;
		}

		public CamPingIndicatorSpawner()
			: this()
		{
		}

		private void Start()
		{
			if (factory != null)
			{
				factory.RegisterPrefab(camPingIndicator, camPingIndicator.get_name(), this.get_gameObject());
			}
			_camPings = new CamPingIndicatorBehaviour[queueSize];
		}

		public void SpawnCamPingIndicator(MapPingBehaviour mapPing, PingType type)
		{
			if (_camPings[_lastFreePosition] != null)
			{
				Object.Destroy(_camPings[_lastFreePosition].get_gameObject());
			}
			_camPings[_lastFreePosition] = factory.Build(camPingIndicator.get_name()).GetComponent<CamPingIndicatorBehaviour>();
			_camPings[_lastFreePosition].mapPingBehaviour = mapPing;
			_camPings[_lastFreePosition].type = type;
			_camPings[_lastFreePosition].divider = radius;
			_lastFreePosition++;
			if (_lastFreePosition > queueSize - 1)
			{
				_lastFreePosition = 0;
			}
		}
	}
}
