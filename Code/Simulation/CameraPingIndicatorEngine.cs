using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation
{
	internal class CameraPingIndicatorEngine : IEngine, ITickable, IInitialize, ITickableBase
	{
		private IPingObjectsManagementComponent _pingObjectsManagementComponent;

		private GameObject[] _cameraPingIndicators;

		private PingType[] _correspondingTypes;

		private GameObject[] _correspondingMapPings;

		private int _lastIndex;

		private const string CAMERA_INDICATOR_NAME = "HUD_Commands_sidescreen";

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
				typeof(ICameraIndicatorDataComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = (component as IPingObjectsManagementComponent);
				_cameraPingIndicators = (GameObject[])new GameObject[_pingObjectsManagementComponent.GetCameraPingIndicatorMaxNumber()];
				_correspondingMapPings = (GameObject[])new GameObject[_pingObjectsManagementComponent.GetCameraPingIndicatorMaxNumber()];
				_correspondingTypes = new PingType[_pingObjectsManagementComponent.GetCameraPingIndicatorMaxNumber()];
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
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _cameraPingIndicators.Length; i++)
			{
				if (!(_cameraPingIndicators[i] != null) || !(_correspondingMapPings[i] != null))
				{
					continue;
				}
				Camera main = Camera.get_main();
				Vector3 val = _correspondingMapPings[i].get_transform().get_position() - main.get_transform().get_position();
				val = main.get_transform().InverseTransformDirection(val);
				Vector3 val2 = Vector3.get_right() * val.x + Vector3.get_up() * 0f + Vector3.get_forward() * val.z;
				Vector3 val3 = Vector3.get_right() * 0f + Vector3.get_up() * val.y + Vector3.get_forward() * val.z;
				Vector3 val4 = default(Vector3);
				val4._002Ector(val.x, val.y, 0f);
				float num = main.get_fieldOfView() * ((float)Math.PI / 180f);
				float num2 = 2f * Mathf.Atan(Mathf.Tan(num / 2f) * main.get_aspect());
				float num3 = 57.29578f * num2;
				float num4 = Vector3.Angle(Vector3.get_forward(), val2);
				float num5 = Vector3.Angle(Vector3.get_forward(), val3);
				if (num4 > num3 / 2f || num5 > main.get_fieldOfView() / 2f)
				{
					_cameraPingIndicators[i].SetActive(true);
					float num6 = Vector3.Angle(Vector3.get_up(), val4);
					float num7 = Vector3.Dot(Vector3.get_right(), val4);
					if (num4 > num6)
					{
						num6 = num4;
					}
					if (num7 > 0f)
					{
						num6 = 0f - num6;
					}
					float num8 = num6;
					float num9 = 1.3f;
					Vector3 zero = Vector3.get_zero();
					if (num9 <= 1f)
					{
						zero = Vector3.get_zero() + Quaternion.Euler(0f, 0f, num6) * (Vector3.get_up() * (float)(Screen.get_height() / 2) * num9);
					}
					else
					{
						if (Mathf.Abs(num8) > 90f)
						{
							num8 = 90f - (Mathf.Abs(num6) - 90f);
						}
						float num10 = (float)(Screen.get_height() / 2) / Mathf.Cos((float)Math.PI / 180f * Mathf.Abs(num8));
						if (num10 > (float)(Screen.get_height() / 2) * num9)
						{
							num10 = (float)(Screen.get_height() / 2) * num9;
						}
						zero = Vector3.get_zero() + Quaternion.Euler(0f, 0f, num6) * (Vector3.get_up() * (num10 - 22f));
					}
					_cameraPingIndicators[i].get_transform().GetChild((int)_correspondingTypes[i]).GetChild(2)
						.set_localRotation(Quaternion.Euler(0f, 0f, 0f - (180f - num6)));
					_cameraPingIndicators[i].get_transform().set_localPosition(zero);
				}
				else
				{
					_cameraPingIndicators[i].SetActive(false);
				}
			}
		}

		private void OnMapPingCreated(GameObject mapPing, PingType type, int playerId)
		{
			if (playerId != playerTeamsContainer.localPlayerId)
			{
				GameObject val = gameObjectFactory.Build("HUD_Commands_sidescreen");
				val.get_transform().GetChild((int)type).get_gameObject()
					.SetActive(true);
				val.SetActive(false);
				if (_cameraPingIndicators[_lastIndex] != null)
				{
					_pingObjectsManagementComponent.RemoveFromCameraPingIndicatorDictionary(_cameraPingIndicators[_lastIndex]);
					Object.Destroy(_cameraPingIndicators[_lastIndex]);
					_correspondingMapPings[_lastIndex] = null;
				}
				_cameraPingIndicators[_lastIndex] = val;
				_correspondingMapPings[_lastIndex] = mapPing;
				_correspondingTypes[_lastIndex] = type;
				_pingObjectsManagementComponent.AddToCameraPingIndicatorDictionary(_cameraPingIndicators[_lastIndex], Time.get_time());
				_lastIndex++;
				if (_lastIndex == _pingObjectsManagementComponent.GetCameraPingIndicatorMaxNumber())
				{
					_lastIndex = 0;
				}
			}
		}
	}
}
