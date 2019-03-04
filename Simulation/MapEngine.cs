using Fabric;
using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MapEngine : IEngine, ITickable, IInitialize, ITickableBase
	{
		private List<IInputPlugin> _inputPlugins;

		private List<string> _fabricEventList;

		private Transform _bottomR;

		private Transform _topL;

		private Vector3 _clickMousePosition;

		private PingType _type = PingType.GOING_HERE;

		private PingType _clickType = PingType.GOING_HERE;

		private Vector2 _pixelOfsset;

		private int _numPingActive;

		private float _halfMapSize;

		private float _halfUnscaledMapSize;

		private float _pingTimer;

		private float _pingLife = 5f;

		private float _pingCooldown = 10f;

		private int _pingNumber = 2;

		private bool _shown;

		private bool _clickPressed;

		private bool _selectorsDisabled;

		private bool _clickOutsideMap;

		private const float CLICK_PRESSED_TIME = 1f;

		[Inject]
		private INetworkEventManagerClient eventManager
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

		[Inject]
		private PlayerNamesContainer playerNamesContainer
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
		private MapDataObserver mapDataObserver
		{
			get;
			set;
		}

		[Inject]
		private MapPingClientCommandObserver clientCommandObserver
		{
			get;
			set;
		}

		private event Action<Vector3, float> ShowPingSelector = delegate
		{
		};

		private event Action<Vector3> HidePingSelector = delegate
		{
		};

		private event Action<PingType, Vector3, string, float> ShowPingMapIndicator = delegate
		{
		};

		private event Action<Vector3, PingType, string, float> ShowPingAtLocation = delegate
		{
		};

		private event Action<bool> ChangeSelectorsColorToGray = delegate
		{
		};

		private event Action<float> SetProgressBar = delegate
		{
		};

		private event Action<float, float> DrawLine = delegate
		{
		};

		private event Action<string, PingType> ShowPingMessage = delegate
		{
		};

		private event Action<bool, PingType> SelectPingType = delegate
		{
		};

		public MapEngine()
		{
			_inputPlugins = new List<IInputPlugin>();
			_fabricEventList = new List<string>();
			_fabricEventList.Add("GUI_MapPing_MoveHere");
			_fabricEventList.Add("GUI_MapPing_GoingHere");
			_fabricEventList.Add("GUI_MapPing_Danger");
		}

		void IInitialize.OnDependenciesInjected()
		{
			mapDataObserver.OnInitializationData += InitializeData;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[6]
			{
				typeof(IMapInputComponent),
				typeof(IMapViewComponent),
				typeof(IMapPingSpawnerComponent),
				typeof(IMapPresenterComponent),
				typeof(IPingMessageDisplayerComponent),
				typeof(IPingTypeComponent)
			};
		}

		public void AddPlugin(IComponent plugin)
		{
			if (plugin is IInputPlugin)
			{
				_inputPlugins.Add((IInputPlugin)plugin);
			}
		}

		public void RemovePlugin(IComponent plugin)
		{
			if (plugin is IInputPlugin)
			{
				_inputPlugins.Remove((IInputPlugin)plugin);
			}
		}

		public void Add(IComponent component)
		{
			if (component is IMapViewComponent)
			{
				(component as IMapViewComponent).InitializeMapSize += InitializeMapSize;
				(component as IMapViewComponent).PingTypeSelected += PingTypeSelected;
				IMapViewComponent obj = component as IMapViewComponent;
				ChangeSelectorsColorToGray += obj.ChangeSelectorsColorToGray;
				IMapViewComponent obj2 = component as IMapViewComponent;
				SetProgressBar += obj2.SetProgressBar;
				IMapViewComponent obj3 = component as IMapViewComponent;
				DrawLine += obj3.DrawLine;
				IMapViewComponent obj4 = component as IMapViewComponent;
				ShowPingSelector += obj4.ShowPingSelector;
				IMapViewComponent obj5 = component as IMapViewComponent;
				HidePingSelector += obj5.HidePingSelector;
				IMapViewComponent obj6 = component as IMapViewComponent;
				ShowPingMapIndicator += obj6.ShowPingIndicator;
			}
			else if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData += HandleInputData;
			}
			else if (component is IMapPingSpawnerComponent)
			{
				(component as IMapPingSpawnerComponent).OnPingDestroyed += OnPingDestroyed;
				IMapPingSpawnerComponent obj7 = component as IMapPingSpawnerComponent;
				ShowPingAtLocation += obj7.ShowPingAtLocation;
				(component as IMapPingSpawnerComponent).InitializePingTimesAndNumber += InitializePingTimesAndNumber;
			}
			else if (component is IMapPresenterComponent)
			{
				(component as IMapPresenterComponent).MapOpened += MapOpened;
				(component as IMapPresenterComponent).MapClosed += MapClosed;
			}
			else if (component is IPingMessageDisplayerComponent)
			{
				IPingMessageDisplayerComponent obj8 = component as IPingMessageDisplayerComponent;
				ShowPingMessage += obj8.ShowPingMessage;
			}
			else if (component is IPingTypeComponent)
			{
				IPingTypeComponent obj9 = component as IPingTypeComponent;
				SelectPingType += obj9.SelectPingType;
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMapViewComponent)
			{
				(component as IMapViewComponent).InitializeMapSize -= InitializeMapSize;
				(component as IMapViewComponent).PingTypeSelected -= PingTypeSelected;
				IMapViewComponent obj = component as IMapViewComponent;
				ChangeSelectorsColorToGray -= obj.ChangeSelectorsColorToGray;
				IMapViewComponent obj2 = component as IMapViewComponent;
				SetProgressBar -= obj2.SetProgressBar;
				IMapViewComponent obj3 = component as IMapViewComponent;
				DrawLine -= obj3.DrawLine;
				IMapViewComponent obj4 = component as IMapViewComponent;
				ShowPingSelector -= obj4.ShowPingSelector;
				IMapViewComponent obj5 = component as IMapViewComponent;
				HidePingSelector -= obj5.HidePingSelector;
				IMapViewComponent obj6 = component as IMapViewComponent;
				ShowPingMapIndicator -= obj6.ShowPingIndicator;
			}
			else if (component is IInputComponent)
			{
				(component as IMapInputComponent).OnInputData -= HandleInputData;
			}
			else if (component is IMapPingSpawnerComponent)
			{
				(component as IMapPingSpawnerComponent).OnPingDestroyed -= OnPingDestroyed;
				IMapPingSpawnerComponent obj7 = component as IMapPingSpawnerComponent;
				ShowPingAtLocation -= obj7.ShowPingAtLocation;
				(component as IMapPingSpawnerComponent).InitializePingTimesAndNumber -= InitializePingTimesAndNumber;
			}
			else if (component is IMapPresenterComponent)
			{
				(component as IMapPresenterComponent).MapOpened -= MapOpened;
				(component as IMapPresenterComponent).MapClosed -= MapClosed;
			}
			else if (component is IPingMessageDisplayerComponent)
			{
				IPingMessageDisplayerComponent obj8 = component as IPingMessageDisplayerComponent;
				ShowPingMessage -= obj8.ShowPingMessage;
			}
			else if (component is IPingTypeComponent)
			{
				IPingTypeComponent obj9 = component as IPingTypeComponent;
				SelectPingType -= obj9.SelectPingType;
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			if (_pingTimer > 0f)
			{
				_pingTimer -= Time.get_deltaTime();
				this.SetProgressBar(1f - _pingTimer / 10f);
			}
			else if (_selectorsDisabled)
			{
				_selectorsDisabled = false;
				this.SetProgressBar(0f);
				this.ChangeSelectorsColorToGray(_selectorsDisabled);
				EventManager.get_Instance().PostEvent("GUI_MapPing_Wheel_ON");
			}
			if (!_clickPressed)
			{
				return;
			}
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - _clickMousePosition.x + _pixelOfsset.x, _clickMousePosition.y - _pixelOfsset.y, _clickMousePosition.z);
			Vector3 mousePosition = Input.get_mousePosition();
			mousePosition._002Ector(mousePosition.x, mousePosition.y - 16f, mousePosition.z);
			Vector3 val2 = default(Vector3);
			val2._002Ector((float)Screen.get_width() - mousePosition.x + _pixelOfsset.x, mousePosition.y - _pixelOfsset.y, mousePosition.z);
			Vector3 val3 = val2 - val;
			val3._002Ector(0f - val3.x, val3.y, val3.z);
			float num = Vector3.Angle(Vector3.get_up(), val3);
			float num2 = Vector3.Dot(Vector3.get_right(), val3);
			float num3 = 1f;
			if (num2 > 0f)
			{
				num = 0f - num;
			}
			num3 = ((!_clickOutsideMap) ? 1.5f : 2f);
			if (val3.get_magnitude() > 16f * (_halfMapSize / _halfUnscaledMapSize) * num3 && _pingTimer <= 0f)
			{
				if (num > 60f)
				{
					this.SelectPingType(arg1: true, PingType.DANGER);
					this.SelectPingType(arg1: false, PingType.MOVE_HERE);
					this.SelectPingType(arg1: false, PingType.GOING_HERE);
				}
				else if (num < -60f)
				{
					this.SelectPingType(arg1: true, PingType.MOVE_HERE);
					this.SelectPingType(arg1: false, PingType.GOING_HERE);
					this.SelectPingType(arg1: false, PingType.DANGER);
				}
				else if (num < 60f && num > -60f)
				{
					this.SelectPingType(arg1: true, PingType.GOING_HERE);
					this.SelectPingType(arg1: false, PingType.MOVE_HERE);
					this.SelectPingType(arg1: false, PingType.DANGER);
				}
			}
			else
			{
				this.SelectPingType(arg1: false, PingType.MOVE_HERE);
				this.SelectPingType(arg1: false, PingType.GOING_HERE);
				this.SelectPingType(arg1: false, PingType.DANGER);
				_type = _clickType;
			}
		}

		private void HandleInputData(InputCharacterData input)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			float num = input.data[16];
			float num2 = input.data[18];
			float num3 = input.data[17];
			if (num != 0f)
			{
				if (!_clickPressed)
				{
					GetClick();
					_clickType = PingType.DANGER;
					_type = _clickType;
				}
				return;
			}
			if (num2 != 0f)
			{
				if (!_clickPressed)
				{
					GetClick();
					_clickType = PingType.GOING_HERE;
					_type = _clickType;
				}
				return;
			}
			if (num3 != 0f)
			{
				if (!_clickPressed)
				{
					GetClick();
					_clickType = PingType.MOVE_HERE;
					_type = _clickType;
				}
				return;
			}
			if (_clickPressed && _numPingActive < _pingNumber && _pingTimer <= 0f && _shown)
			{
				if (!_clickOutsideMap)
				{
					Vector3 mousePosition = Input.get_mousePosition();
					mousePosition._002Ector(mousePosition.x, mousePosition.y - 16f, mousePosition.z);
					mousePosition._002Ector((float)Screen.get_width() - mousePosition.x + _pixelOfsset.x, mousePosition.y - _pixelOfsset.y, mousePosition.z);
					Vector3 clickMousePosition = _clickMousePosition;
					clickMousePosition._002Ector((float)Screen.get_width() - clickMousePosition.x + _pixelOfsset.x, clickMousePosition.y - _pixelOfsset.y, clickMousePosition.z);
					float num4 = clickMousePosition.x / (_halfMapSize * 2f);
					float num5 = clickMousePosition.y / (_halfMapSize * 2f);
					Vector3 position = _topL.get_position();
					float x = position.x;
					Vector3 position2 = _bottomR.get_position();
					float num6 = (x - position2.x) * num4;
					Vector3 position3 = _bottomR.get_position();
					float num7 = num6 + position3.x;
					Vector3 position4 = _topL.get_position();
					float z = position4.z;
					Vector3 position5 = _bottomR.get_position();
					float num8 = (z - position5.z) * num5;
					Vector3 position6 = _bottomR.get_position();
					Ray val = default(Ray);
					val._002Ector(new Vector3(num7, 1000000f, num8 + position6.z), Vector3.get_down());
					int num9 = 65536;
					num9 += 131072;
					RaycastHit val2 = default(RaycastHit);
					Physics.Raycast(val, ref val2, 1E+09f, num9);
					Vector3 position7 = _topL.get_position();
					float x2 = position7.x;
					Vector3 position8 = _bottomR.get_position();
					float num10 = (x2 - position8.x) * num4;
					Vector3 position9 = _bottomR.get_position();
					float num11 = num10 + position9.x;
					Vector3 point = val2.get_point();
					float y = point.y;
					Vector3 position10 = _topL.get_position();
					float z2 = position10.z;
					Vector3 position11 = _bottomR.get_position();
					float num12 = (z2 - position11.z) * num5;
					Vector3 position12 = _bottomR.get_position();
					Vector3 pingPosition = default(Vector3);
					pingPosition._002Ector(num11, y, num12 + position12.z);
					float x3 = pingPosition.x;
					Vector3 position13 = _bottomR.get_position();
					if (x3 < position13.x)
					{
						float x4 = pingPosition.x;
						Vector3 position14 = _topL.get_position();
						if (x4 > position14.x)
						{
							float z3 = pingPosition.z;
							Vector3 position15 = _bottomR.get_position();
							if (z3 > position15.z)
							{
								float z4 = pingPosition.z;
								Vector3 position16 = _topL.get_position();
								if (z4 < position16.z)
								{
									ShowPingClickInsideMap(pingPosition);
								}
							}
						}
					}
				}
				else
				{
					Ray val3 = Camera.get_main().ScreenPointToRay(_clickMousePosition);
					int num13 = 65536;
					num13 += 131072;
					RaycastHit val4 = default(RaycastHit);
					if (Physics.Raycast(val3, ref val4, 1000f, num13))
					{
						Vector3 point2 = val4.get_point();
						float z5 = point2.z;
						Vector3 position17 = _topL.get_position();
						if (z5 < position17.z)
						{
							Vector3 point3 = val4.get_point();
							float x5 = point3.x;
							Vector3 position18 = _bottomR.get_position();
							if (x5 < position18.x)
							{
								Vector3 point4 = val4.get_point();
								float x6 = point4.x;
								Vector3 position19 = _topL.get_position();
								if (x6 > position19.x)
								{
									Vector3 point5 = val4.get_point();
									float z6 = point5.z;
									Vector3 position20 = _bottomR.get_position();
									if (z6 > position20.z)
									{
										ShowPingClickOutsideMap(val4.get_point());
										goto IL_04ad;
									}
								}
							}
						}
					}
					EventManager.get_Instance().PostEvent("GUI_MapPing_Failed", 0);
				}
			}
			goto IL_04ad;
			IL_04ad:
			if (_clickPressed)
			{
				this.HidePingSelector(Vector3.get_zero());
				_clickPressed = false;
			}
		}

		public void ShowPing(Vector3 location, PingType type, string user)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			this.ShowPingAtLocation(location, type, user, _pingLife);
			this.ShowPingMessage(user, type);
			Vector2 zero = Vector2.get_zero();
			float x = location.x;
			Vector3 position = _bottomR.get_position();
			float num = x - position.x;
			Vector3 position2 = _topL.get_position();
			float x2 = position2.x;
			Vector3 position3 = _bottomR.get_position();
			zero.x = num / (x2 - position3.x);
			float z = location.z;
			Vector3 position4 = _bottomR.get_position();
			float num2 = z - position4.z;
			Vector3 position5 = _topL.get_position();
			float z2 = position5.z;
			Vector3 position6 = _bottomR.get_position();
			zero.y = num2 / (z2 - position6.z);
			Vector3 arg = default(Vector3);
			arg._002Ector(zero.x * _halfUnscaledMapSize * 2f, zero.y * _halfUnscaledMapSize * 2f);
			this.ShowPingMapIndicator(type, arg, user, _pingLife);
			EventManager.get_Instance().PostEvent(_fabricEventList[(int)type], 0);
		}

		private void ShowPingClickInsideMap(Vector3 pingPosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			this.ShowPingAtLocation(pingPosition, _type, playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _pingLife);
			this.ShowPingMessage(playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _type);
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - _clickMousePosition.x + _pixelOfsset.x, _clickMousePosition.y - _pixelOfsset.y, _clickMousePosition.z);
			this.ShowPingMapIndicator(_type, new Vector3(val.x / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.y / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.z), playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _pingLife);
			EventManager.get_Instance().PostEvent(_fabricEventList[(int)_type], 0);
			_numPingActive++;
			if (_numPingActive == _pingNumber)
			{
				_pingTimer = _pingCooldown;
				_selectorsDisabled = true;
				this.SetProgressBar(0f);
				this.ChangeSelectorsColorToGray(_selectorsDisabled);
				mapPingCooldownObserver.BeginCooldown(_pingCooldown);
				_numPingActive = 0;
			}
			eventManager.SendEventToServer(NetworkEvent.MapPingEvent, new MapPingEventDependency(playerTeamsContainer.localPlayerId, playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerTeamsContainer.localPlayerId), _type, pingPosition));
		}

		private void ShowPingClickOutsideMap(Vector3 pingPosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			this.ShowPingAtLocation(pingPosition, _type, playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _pingLife);
			this.ShowPingMessage(playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _type);
			Vector2 zero = Vector2.get_zero();
			float x = pingPosition.x;
			Vector3 position = _bottomR.get_position();
			float num = x - position.x;
			Vector3 position2 = _topL.get_position();
			float x2 = position2.x;
			Vector3 position3 = _bottomR.get_position();
			zero.x = num / (x2 - position3.x);
			float z = pingPosition.z;
			Vector3 position4 = _bottomR.get_position();
			float num2 = z - position4.z;
			Vector3 position5 = _topL.get_position();
			float z2 = position5.z;
			Vector3 position6 = _bottomR.get_position();
			zero.y = num2 / (z2 - position6.z);
			Vector3 arg = default(Vector3);
			arg._002Ector(zero.x * _halfUnscaledMapSize * 2f, zero.y * _halfUnscaledMapSize * 2f);
			this.ShowPingMapIndicator(_type, arg, playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId), _pingLife);
			EventManager.get_Instance().PostEvent(_fabricEventList[(int)_type], 0);
			_numPingActive++;
			if (_numPingActive == _pingNumber)
			{
				_pingTimer = _pingCooldown;
				_selectorsDisabled = true;
				this.SetProgressBar(0f);
				this.ChangeSelectorsColorToGray(_selectorsDisabled);
				mapPingCooldownObserver.BeginCooldown(_pingCooldown);
				_numPingActive = 0;
			}
			eventManager.SendEventToServer(NetworkEvent.MapPingEvent, new MapPingEventDependency(playerTeamsContainer.localPlayerId, playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerTeamsContainer.localPlayerId), _type, pingPosition));
		}

		private void GetClick()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			_clickPressed = true;
			_clickMousePosition = Input.get_mousePosition();
			_clickMousePosition = new Vector3(_clickMousePosition.x, _clickMousePosition.y - 16f, _clickMousePosition.z);
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - _clickMousePosition.x + _pixelOfsset.x, _clickMousePosition.y - _pixelOfsset.y, _clickMousePosition.z);
			if (val.x > _halfMapSize * 2f || val.y > _halfMapSize * 2f)
			{
				this.ShowPingSelector(new Vector3(val.x / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.y / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.z), 2f);
				_clickOutsideMap = true;
			}
			else
			{
				this.ShowPingSelector(new Vector3(val.x / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.y / (_halfMapSize * 2f) * _halfUnscaledMapSize * 2f, val.z), 1.5f);
				_clickOutsideMap = false;
			}
			if (_pingTimer > 0f)
			{
				EventManager.get_Instance().PostEvent("GUI_MapPing_Wheel_OFF", 0);
			}
			else
			{
				EventManager.get_Instance().PostEvent("GUI_MapPing_Wheel_ON", 0);
			}
		}

		private void InitializeData(Transform bottomR, Transform topL)
		{
			_bottomR = bottomR;
			_topL = topL;
		}

		private void InitializeMapSize(float halfMapSize, float halfUnscaledMapSize, Vector2 pixelOffset)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_halfMapSize = halfMapSize;
			_halfUnscaledMapSize = halfUnscaledMapSize;
			_pixelOfsset = pixelOffset;
		}

		private void OnPingDestroyed()
		{
			if (_numPingActive > 0)
			{
				_numPingActive--;
			}
		}

		private void PingTypeSelected(PingType type)
		{
			if (type == PingType.UNKNOWN)
			{
				_type = _clickType;
			}
			else
			{
				_type = type;
			}
		}

		private void MapOpened()
		{
			_shown = true;
		}

		private void MapClosed()
		{
			_shown = false;
		}

		private void InitializePingTimesAndNumber(float pingLife, float pingCooldown, int pingNumber)
		{
			_pingLife = pingLife;
			_pingCooldown = pingCooldown;
			_pingNumber = pingNumber;
		}
	}
}
