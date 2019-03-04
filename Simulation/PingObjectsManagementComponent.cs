using Svelto.ES.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingObjectsManagementComponent : IPingObjectsManagementComponent, IComponent
	{
		private int _maxMapPingNumber = 2;

		private int _cameraPingIndicatorMaxNumber = 3;

		private int _currentMapPingNumber;

		private float _pingObjectLifetime = 10f;

		private float _mapPingCooldown = 10f;

		private float _currentMapPingTimerValue;

		private Dictionary<GameObject, float> _mapPingTime = new Dictionary<GameObject, float>();

		private Dictionary<GameObject, int> _mapPingCreator = new Dictionary<GameObject, int>();

		private Dictionary<GameObject, float> _pingIndicatorTime = new Dictionary<GameObject, float>();

		private Dictionary<GameObject, float> _cameraPingIndicatorTime = new Dictionary<GameObject, float>();

		public int GetMapPingMaxNumber()
		{
			return _maxMapPingNumber;
		}

		public int GetCameraPingIndicatorMaxNumber()
		{
			return _cameraPingIndicatorMaxNumber;
		}

		public int GetMapPingCurrentNumber()
		{
			return _currentMapPingNumber;
		}

		public float GetPingObjectLifeTime()
		{
			return _pingObjectLifetime;
		}

		public float GetMapPingCooldown()
		{
			return _mapPingCooldown;
		}

		public float GetMapPingCurrentTimerValue()
		{
			return _currentMapPingTimerValue;
		}

		public Dictionary<GameObject, float> GetMapPingTimeDictionary()
		{
			return _mapPingTime;
		}

		public Dictionary<GameObject, int> GetMapPingCreatorDictionary()
		{
			return _mapPingCreator;
		}

		public Dictionary<GameObject, float> GetPingIndicatorTimeDictionary()
		{
			return _pingIndicatorTime;
		}

		public Dictionary<GameObject, float> GetCameraPingIndicatorTimeDictionary()
		{
			return _cameraPingIndicatorTime;
		}

		public void SetMapPingMaxNumber(int maxNumber)
		{
			_maxMapPingNumber = maxNumber;
		}

		public void SetMapPingCurrentNumber(int currentNumber)
		{
			_currentMapPingNumber = currentNumber;
		}

		public void SetPingObjectLifeTime(float lifeTime)
		{
			_pingObjectLifetime = lifeTime;
		}

		public void SetMapPingCooldown(float coolDown)
		{
			_mapPingCooldown = coolDown;
		}

		public void SetMapPingCurrentTimerValue(float currentValue)
		{
			_currentMapPingTimerValue = currentValue;
		}

		public void RemoveFromMapPingTimeDictionary(GameObject mapPing)
		{
			_mapPingTime.Remove(mapPing);
		}

		public void RemoveFromMapPingCreatorDictionary(GameObject mapPing)
		{
			_mapPingCreator.Remove(mapPing);
		}

		public void RemoveFromPingIndicatorDictionary(GameObject pingIndicator)
		{
			_pingIndicatorTime.Remove(pingIndicator);
		}

		public void RemoveFromCameraPingIndicatorDictionary(GameObject cameraPingIndicator)
		{
			_cameraPingIndicatorTime.Remove(cameraPingIndicator);
		}

		public void AddToMapPingTimeDictionary(GameObject mapPing, float creationTime)
		{
			_mapPingTime.Add(mapPing, creationTime);
		}

		public void AddToMapPingCreatorDictionary(GameObject mapPing, int creatorId)
		{
			_mapPingCreator.Add(mapPing, creatorId);
		}

		public void AddToPingIndicatorDictionary(GameObject pingIndicator, float creationTime)
		{
			_pingIndicatorTime.Add(pingIndicator, creationTime);
		}

		public void AddToCameraPingIndicatorDictionary(GameObject cameraPingIndicator, float creationTime)
		{
			_cameraPingIndicatorTime.Add(cameraPingIndicator, creationTime);
		}
	}
}
