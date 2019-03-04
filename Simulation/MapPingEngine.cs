using Fabric;
using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class MapPingEngine : IEngine, IInitialize
	{
		private PingType _clickPingType = PingType.UNKNOWN;

		private Vector3 _clickMousePosition;

		private Transform _bottomR;

		private Transform _topL;

		private string[] fabricEvents = new string[3];

		private bool _clickPressed;

		private IMiniMapViewComponent _minimapViewComponent;

		private IPingSelectorComponent _pingSelectorComponent;

		private IPingPrefabsHolderComponent _pingPrefabsHolderComponent;

		private IPingObjectsManagementComponent _pingObjectsManagementComponent;

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private PlayerNamesContainer playerNamesContainer
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
		private GameObjectPool gameObjectPool
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
		private MapPingCreationObserver mapPingCreationObserver
		{
			get;
			set;
		}

		[Inject]
		private INetworkEventManagerClient eventManager
		{
			get;
			set;
		}

		[Inject]
		private MapPingClientCommandObserver mapPingClientCommandObserver
		{
			get;
			set;
		}

		public MapPingEngine()
		{
			fabricEvents[0] = "GUI_MapPing_MoveHere";
			fabricEvents[1] = "GUI_MapPing_GoingHere";
			fabricEvents[2] = "GUI_MapPing_Danger";
		}

		void IInitialize.OnDependenciesInjected()
		{
			mapDataObserver.OnInitializationData += OnInitializationData;
			mapPingClientCommandObserver.ShowPing += CreatePingFromServer;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[5]
			{
				typeof(IMiniMapViewComponent),
				typeof(IMapInputComponent),
				typeof(IPingSelectorComponent),
				typeof(IPingPrefabsHolderComponent),
				typeof(IPingObjectsManagementComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData += HandleInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = (component as IPingSelectorComponent);
			}
			else if (component is IPingPrefabsHolderComponent)
			{
				_pingPrefabsHolderComponent = (component as IPingPrefabsHolderComponent);
			}
			else if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = (component as IPingObjectsManagementComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData -= HandleInputData;
			}
			else if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
			else if (component is IPingSelectorComponent)
			{
				_pingSelectorComponent = null;
			}
			else if (component is IPingPrefabsHolderComponent)
			{
				_pingPrefabsHolderComponent = null;
			}
			else if (component is IPingObjectsManagementComponent)
			{
				_pingObjectsManagementComponent = null;
			}
		}

		private void HandleInputData(InputCharacterData input)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			float num = input.data[16];
			float num2 = input.data[17];
			float num3 = input.data[18];
			if (_minimapViewComponent.GetIsPingContextActive() && _minimapViewComponent.GetCanPing())
			{
				if (num > 0f)
				{
					if (!_clickPressed)
					{
						_clickPingType = PingType.DANGER;
						_clickPressed = true;
						_clickMousePosition = Input.get_mousePosition();
					}
				}
				else if (num2 > 0f)
				{
					if (!_clickPressed)
					{
						_clickPingType = PingType.MOVE_HERE;
						_clickPressed = true;
						_clickMousePosition = Input.get_mousePosition();
					}
				}
				else if (num3 > 0f)
				{
					if (!_clickPressed)
					{
						_clickPingType = PingType.GOING_HERE;
						_clickPressed = true;
						_clickMousePosition = Input.get_mousePosition();
					}
				}
				else if (_clickPressed && _pingObjectsManagementComponent.GetMapPingCurrentTimerValue() <= 0f)
				{
					_clickPressed = false;
					if (CheckIfOutsideMap(_clickMousePosition))
					{
						CreatePingOnOutsideMapClick();
					}
					else
					{
						SwitchBasedOnDebugMode();
					}
				}
				else
				{
					_clickPressed = false;
				}
			}
			else if (_clickPressed)
			{
				_clickPressed = false;
			}
		}

		private void SwitchBasedOnDebugMode()
		{
			CreatePingOnInsideMapClick();
		}

		private void CreatePingOnInsideMapClick()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			float scaledHalfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
			Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
			Vector3 clickMousePosition = _clickMousePosition;
			clickMousePosition._002Ector((float)Screen.get_width() - clickMousePosition.x + pixelOffset.x, clickMousePosition.y - pixelOffset.y, clickMousePosition.z);
			float num = clickMousePosition.x / (scaledHalfMapSize * 2f);
			float num2 = clickMousePosition.y / (scaledHalfMapSize * 2f);
			Vector3 position = _topL.get_position();
			float x = position.x;
			Vector3 position2 = _bottomR.get_position();
			float num3 = (x - position2.x) * num;
			Vector3 position3 = _bottomR.get_position();
			float num4 = num3 + position3.x;
			Vector3 position4 = _topL.get_position();
			float z = position4.z;
			Vector3 position5 = _bottomR.get_position();
			float num5 = (z - position5.z) * num2;
			Vector3 position6 = _bottomR.get_position();
			Ray val = default(Ray);
			val._002Ector(new Vector3(num4, 1000000f, num5 + position6.z), Vector3.get_down());
			int num6 = 65536;
			num6 += 131072;
			RaycastHit val2 = default(RaycastHit);
			Physics.Raycast(val, ref val2, 1000000f, num6);
			Vector3 position7 = _topL.get_position();
			float x2 = position7.x;
			Vector3 position8 = _bottomR.get_position();
			float num7 = (x2 - position8.x) * num;
			Vector3 position9 = _bottomR.get_position();
			float num8 = num7 + position9.x;
			Vector3 point = val2.get_point();
			float y = point.y;
			Vector3 position10 = _topL.get_position();
			float z2 = position10.z;
			Vector3 position11 = _bottomR.get_position();
			float num9 = (z2 - position11.z) * num2;
			Vector3 position12 = _bottomR.get_position();
			Vector3 val3 = default(Vector3);
			val3._002Ector(num8, y, num9 + position12.z);
			float x3 = val3.x;
			Vector3 position13 = _bottomR.get_position();
			if (!(x3 < position13.x))
			{
				return;
			}
			float x4 = val3.x;
			Vector3 position14 = _topL.get_position();
			if (!(x4 > position14.x))
			{
				return;
			}
			float z3 = val3.z;
			Vector3 position15 = _bottomR.get_position();
			if (!(z3 > position15.z))
			{
				return;
			}
			float z4 = val3.z;
			Vector3 position16 = _topL.get_position();
			if (z4 < position16.z)
			{
				PingType pingType = _pingSelectorComponent.GetSelectedPingType();
				if (pingType == PingType.UNKNOWN)
				{
					pingType = _clickPingType;
				}
				GameObject val4 = gameObjectFactory.Build(_pingPrefabsHolderComponent.GetPingPrefabOfType(pingType).get_name());
				val4.get_transform().set_position(val3);
				val4.GetComponent<MapPingComponent>().SetMapPingLabel(playerNamesContainer.GetDisplayName(playerTeamsContainer.localPlayerId));
				mapPingCreationObserver.MapPingCreated(val4, pingType, playerTeamsContainer.localPlayerId);
				EventManager.get_Instance().PostEvent(fabricEvents[(int)pingType], 0);
				eventManager.SendEventToServer(NetworkEvent.MapPingEvent, new MapPingEventDependency(playerTeamsContainer.localPlayerId, playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerTeamsContainer.localPlayerId), pingType, val3));
			}
		}

		private void CreatePingOnOutsideMapClick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			Ray val = Camera.get_main().ScreenPointToRay(_clickMousePosition);
			RaycastHit val2 = default(RaycastHit);
			if (Physics.Raycast(val, ref val2, 1000f, GameLayers.MAP_PING_LAYER_MASK))
			{
				if (val2.get_transform().get_gameObject().get_layer() != GameLayers.LEVEL_BARRIER)
				{
					PingType pingType = _pingSelectorComponent.GetSelectedPingType();
					if (pingType == PingType.UNKNOWN)
					{
						pingType = _clickPingType;
					}
					GameObject val3 = gameObjectFactory.Build(_pingPrefabsHolderComponent.GetPingPrefabOfType(pingType).get_name());
					val3.get_transform().set_position(val2.get_point());
					val3.GetComponent<MapPingComponent>().SetMapPingLabel(playerNamesContainer.GetDisplayName(playerTeamsContainer.localPlayerId));
					mapPingCreationObserver.MapPingCreated(val3, pingType, playerTeamsContainer.localPlayerId);
					EventManager.get_Instance().PostEvent(fabricEvents[(int)pingType], 0);
					eventManager.SendEventToServer(NetworkEvent.MapPingEvent, new MapPingEventDependency(playerTeamsContainer.localPlayerId, playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerTeamsContainer.localPlayerId), pingType, val2.get_point()));
				}
			}
			else
			{
				EventManager.get_Instance().PostEvent("GUI_MapPing_Failed", 0);
			}
		}

		private void CreatePingFromServer(Vector3 pingPosition, PingType type, int senderId)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build(_pingPrefabsHolderComponent.GetPingPrefabOfType(type).get_name());
			val.get_transform().set_position(pingPosition);
			val.GetComponent<MapPingComponent>().SetMapPingLabel(playerNamesContainer.GetDisplayName(senderId));
			mapPingCreationObserver.MapPingCreated(val, type, senderId);
			EventManager.get_Instance().PostEvent(fabricEvents[(int)type], 0);
		}

		private bool CheckIfOutsideMap(Vector3 clickMousePosition)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			float scaledHalfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
			Vector2 pixelOffset = _minimapViewComponent.GetPixelOffset();
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - clickMousePosition.x + pixelOffset.x, clickMousePosition.y - pixelOffset.y, clickMousePosition.z);
			if (val.x > scaledHalfMapSize * 2f || val.y > scaledHalfMapSize * 2f)
			{
				return true;
			}
			return false;
		}

		private void OnInitializationData(Transform bottomR, Transform topL)
		{
			_bottomR = bottomR;
			_topL = topL;
		}
	}
}
