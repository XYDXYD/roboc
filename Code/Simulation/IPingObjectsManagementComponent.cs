using Svelto.ES.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal interface IPingObjectsManagementComponent : IComponent
	{
		int GetMapPingMaxNumber();

		int GetCameraPingIndicatorMaxNumber();

		int GetMapPingCurrentNumber();

		float GetPingObjectLifeTime();

		float GetMapPingCooldown();

		float GetMapPingCurrentTimerValue();

		Dictionary<GameObject, float> GetMapPingTimeDictionary();

		Dictionary<GameObject, int> GetMapPingCreatorDictionary();

		Dictionary<GameObject, float> GetPingIndicatorTimeDictionary();

		Dictionary<GameObject, float> GetCameraPingIndicatorTimeDictionary();

		void SetMapPingMaxNumber(int maxNumber);

		void SetMapPingCurrentNumber(int currentNumber);

		void SetPingObjectLifeTime(float lifeTime);

		void SetMapPingCooldown(float coolDown);

		void SetMapPingCurrentTimerValue(float currentValue);

		void RemoveFromMapPingTimeDictionary(GameObject mapPing);

		void RemoveFromMapPingCreatorDictionary(GameObject mapPing);

		void RemoveFromPingIndicatorDictionary(GameObject pingIndicator);

		void RemoveFromCameraPingIndicatorDictionary(GameObject cameraPingIndicator);

		void AddToMapPingTimeDictionary(GameObject mapPing, float creationTime);

		void AddToMapPingCreatorDictionary(GameObject mapPing, int creatorId);

		void AddToPingIndicatorDictionary(GameObject pingIndicator, float creationTime);

		void AddToCameraPingIndicatorDictionary(GameObject cameraPingIndicator, float creationTime);
	}
}
