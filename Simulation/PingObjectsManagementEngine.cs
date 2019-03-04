using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingObjectsManagementEngine : IEngine, ITickable, IInitialize, ITickableBase
	{
		private IPingObjectsManagementComponent _pingObjectsManagementComponent;

		private List<GameObject> _indicatorsToRemove = new List<GameObject>();

		[Inject]
		private GameObjectPool gameObjectPool
		{
			get;
			set;
		}

		[Inject]
		private MapPingCooldownObserver mapPingCooldownObserver
		{
			get;
			set;
		}

		[Inject]
		private PingIndicatorCreationObserver pingIndicatorCreationObserver
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			pingIndicatorCreationObserver.OnPingIndicatorCreated += OnPingIndicatorCreated;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[1]
			{
				typeof(IPingObjectsManagementComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = (component as IPingObjectsManagementComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = null;
			}
		}

		public void Tick(float deltaSec)
		{
			if (_pingObjectsManagementComponent.GetMapPingCurrentTimerValue() > 0f)
			{
				_pingObjectsManagementComponent.SetMapPingCurrentTimerValue(_pingObjectsManagementComponent.GetMapPingCurrentTimerValue() - deltaSec);
			}
			CheckPingIndicatorLife();
			CheckCameraPingIndicatorLife();
		}

		private void CheckCameraPingIndicatorLife()
		{
			Dictionary<GameObject, float> cameraPingIndicatorTimeDictionary = _pingObjectsManagementComponent.GetCameraPingIndicatorTimeDictionary();
			RemovePingIndicators(cameraPingIndicatorTimeDictionary);
		}

		private void CheckPingIndicatorLife()
		{
			Dictionary<GameObject, float> pingIndicatorTimeDictionary = _pingObjectsManagementComponent.GetPingIndicatorTimeDictionary();
			RemovePingIndicators(pingIndicatorTimeDictionary);
		}

		private void RemovePingIndicators(Dictionary<GameObject, float> pingIndicatorDictionary)
		{
			if (pingIndicatorDictionary.Count <= 0)
			{
				return;
			}
			float time = Time.get_time();
			foreach (KeyValuePair<GameObject, float> item in pingIndicatorDictionary)
			{
				if (item.Key != null && time - item.Value >= _pingObjectsManagementComponent.GetPingObjectLifeTime())
				{
					_indicatorsToRemove.Add(item.Key);
				}
			}
			if (_indicatorsToRemove.Count > 0)
			{
				for (int i = 0; i < _indicatorsToRemove.Count; i++)
				{
					GameObject val = _indicatorsToRemove[i];
					pingIndicatorDictionary.Remove(val);
					Object.Destroy(val);
				}
				_indicatorsToRemove.Clear();
			}
		}

		private void OnPingIndicatorCreated(GameObject pingIndicator)
		{
			_pingObjectsManagementComponent.AddToPingIndicatorDictionary(pingIndicator, Time.get_time());
		}
	}
}
