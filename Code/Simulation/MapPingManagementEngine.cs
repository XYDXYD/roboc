using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MapPingManagementEngine : IEngine, ITickable, IInitialize, ITickableBase
	{
		private IPingObjectsManagementComponent _pingObjectsManagementComponent;

		private List<IMapPingObjectComponent> _mapPingObjectComponents = new List<IMapPingObjectComponent>();

		private List<GameObject> _mapPingKeysToRemove = new List<GameObject>();

		[Inject]
		private MapPingCreationObserver mapPingCreationObserver
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
		private GameObjectPool gameObjectPool
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			mapPingCreationObserver.OnMapPingCreated += OnMapPingCreated;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[2]
			{
				typeof(IPingObjectsManagementComponent),
				typeof(IMapPingObjectComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = (component as IPingObjectsManagementComponent);
			}
			else if (component is IMapPingObjectComponent)
			{
				_mapPingObjectComponents.Add(component as IMapPingObjectComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = null;
			}
			else if (component is IMapPingObjectComponent)
			{
				_mapPingObjectComponents.Remove(component as IMapPingObjectComponent);
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			CheckMapPingLife();
			if (_mapPingObjectComponents.Count > 0)
			{
				List<IMapPingObjectComponent> list = new List<IMapPingObjectComponent>(_mapPingObjectComponents);
				foreach (IMapPingObjectComponent item in list)
				{
					float cameraDistance = item.GetCameraDistance();
					float scalingUpPercentage = item.GetScalingUpPercentage();
					float decresingColorPercentage = item.GetDecresingColorPercentage();
					if (scalingUpPercentage > 0f)
					{
						float num = 1f + scalingUpPercentage * Mathf.Abs(cameraDistance);
						item.SetMapPingScale(item.GetMapPingInitialScale() * num);
					}
					if (decresingColorPercentage > 0f)
					{
						SetPingColor(cameraDistance, item, decresingColorPercentage);
					}
				}
			}
		}

		private void CheckMapPingLife()
		{
			if (_pingObjectsManagementComponent.GetMapPingCurrentNumber() >= _pingObjectsManagementComponent.GetMapPingMaxNumber())
			{
				_pingObjectsManagementComponent.SetMapPingCurrentNumber(0);
			}
			Dictionary<GameObject, float> mapPingTimeDictionary = _pingObjectsManagementComponent.GetMapPingTimeDictionary();
			Dictionary<GameObject, int> mapPingCreatorDictionary = _pingObjectsManagementComponent.GetMapPingCreatorDictionary();
			if (mapPingTimeDictionary.Count > 0)
			{
				float time = Time.get_time();
				foreach (KeyValuePair<GameObject, float> item in mapPingTimeDictionary)
				{
					if (time - item.Value >= _pingObjectsManagementComponent.GetPingObjectLifeTime())
					{
						_mapPingKeysToRemove.Add(item.Key);
						if (playerTeamsContainer.localPlayerId == mapPingCreatorDictionary[item.Key] && _pingObjectsManagementComponent.GetMapPingCurrentNumber() > 0)
						{
							_pingObjectsManagementComponent.SetMapPingCurrentNumber(_pingObjectsManagementComponent.GetMapPingCurrentNumber() - 1);
						}
					}
				}
				for (int i = 0; i < _mapPingKeysToRemove.Count; i++)
				{
					GameObject val = _mapPingKeysToRemove[i];
					_pingObjectsManagementComponent.RemoveFromMapPingTimeDictionary(val);
					_pingObjectsManagementComponent.RemoveFromMapPingCreatorDictionary(val);
					Object.Destroy(val);
				}
				_mapPingKeysToRemove.Clear();
			}
		}

		private void SetPingColor(float distanceToCamera, IMapPingObjectComponent mapPingComponent, float colorDecreasingPercentage)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Color mapPingInitialColor = mapPingComponent.GetMapPingInitialColor();
			Color color = default(Color);
			color._002Ector(mapPingInitialColor.r - colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * mapPingInitialColor.r, mapPingInitialColor.g - colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * mapPingInitialColor.g, mapPingInitialColor.b - colorDecreasingPercentage * Mathf.Abs(distanceToCamera) * mapPingInitialColor.b);
			Color transparentColor = default(Color);
			transparentColor._002Ector(color.r, color.g, color.b, 0.5f);
			mapPingComponent.SetMapPingColor(color, transparentColor);
		}

		private void OnMapPingCreated(GameObject mapPing, PingType type, int playerId)
		{
			_pingObjectsManagementComponent.AddToMapPingTimeDictionary(mapPing, Time.get_time());
			_pingObjectsManagementComponent.AddToMapPingCreatorDictionary(mapPing, playerId);
			if (playerId == playerTeamsContainer.localPlayerId)
			{
				_pingObjectsManagementComponent.SetMapPingCurrentNumber(_pingObjectsManagementComponent.GetMapPingCurrentNumber() + 1);
				if (_pingObjectsManagementComponent.GetMapPingCurrentNumber() >= _pingObjectsManagementComponent.GetMapPingMaxNumber())
				{
					_pingObjectsManagementComponent.SetMapPingCurrentTimerValue(_pingObjectsManagementComponent.GetMapPingCooldown());
					mapPingCooldownObserver.BeginCooldown(_pingObjectsManagementComponent.GetMapPingCooldown());
				}
			}
		}
	}
}
