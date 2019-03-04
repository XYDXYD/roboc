using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class PingIndicatorEngine : IEngine, IInitialize, IWaitForFrameworkDestruction
	{
		private IMiniMapViewComponent _minimapViewComponent;

		private Transform _bottomR;

		private Transform _topL;

		[Inject]
		private MapPingCreationObserver mapPingCreationObserver
		{
			get;
			set;
		}

		[Inject]
		private IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		private MapDataObserver mapDataObserver
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

		public Type[] AcceptedComponents()
		{
			return new Type[1]
			{
				typeof(IMiniMapViewComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			mapPingCreationObserver.OnMapPingCreated += OnMapPingCreated;
			mapDataObserver.OnInitializationData += OnInitializationData;
		}

		private void OnMapPingCreated(GameObject mapPing, PingType type, int playerId)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build(_minimapViewComponent.GetPingIndicatorNameOfType(type));
			val.SetActive(true);
			Vector3 position = mapPing.get_transform().get_position();
			float unscaledHalfMapSize = _minimapViewComponent.GetUnscaledHalfMapSize();
			Vector2 zero = Vector2.get_zero();
			float x = position.x;
			Vector3 position2 = _bottomR.get_position();
			float num = x - position2.x;
			Vector3 position3 = _topL.get_position();
			float x2 = position3.x;
			Vector3 position4 = _bottomR.get_position();
			zero.x = num / (x2 - position4.x);
			float z = position.z;
			Vector3 position5 = _bottomR.get_position();
			float num2 = z - position5.z;
			Vector3 position6 = _topL.get_position();
			float z2 = position6.z;
			Vector3 position7 = _bottomR.get_position();
			zero.y = num2 / (z2 - position7.z);
			Vector3 localPosition = default(Vector3);
			localPosition._002Ector(0f - zero.x * unscaledHalfMapSize * 2f, zero.y * unscaledHalfMapSize * 2f);
			val.get_transform().set_localPosition(localPosition);
			pingIndicatorCreationObserver.PingIndicatorCreated(val);
		}

		private void OnInitializationData(Transform bottomR, Transform topL)
		{
			_bottomR = bottomR;
			_topL = topL;
		}

		public void OnFrameworkDestroyed()
		{
			mapPingCreationObserver.OnMapPingCreated -= OnMapPingCreated;
			mapDataObserver.OnInitializationData -= OnInitializationData;
		}
	}
}
