using Fabric;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class ShowMapEngine : IEngine, ITickable, IInitialize, ITickableBase
	{
		private IMiniMapViewComponent _minimapViewComponent;

		private IMapModeComponent _mapModeComponent;

		private Vector2 _pixelOffset;

		private List<string> _fabricEventList;

		private float _closeTimer;

		private bool _shown;

		private bool _contextChanged;

		private bool _canPing = true;

		private bool _previousIsZoomed;

		private float _halfMapSize;

		private const string FABRIC_OPEN_MAP = "GUI_MapPing_Open";

		private const string FABRIC_CLOSE_MAP = "GUI_MapPing_Close";

		[Inject]
		private ICursorMode cursorMode
		{
			get;
			set;
		}

		[Inject]
		private MachineSpawnDispatcher machineSpawnDispatcher
		{
			get;
			set;
		}

		[Inject]
		private DestructionReporter destructionReporter
		{
			get;
			set;
		}

		[Inject]
		private GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineSpawnDispatcher.OnPlayerRespawnedIn += OnRespawnedIn;
			destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[3]
			{
				typeof(IMiniMapViewComponent),
				typeof(IMapInputComponent),
				typeof(IMapModeComponent)
			};
		}

		public void Add(IComponent component)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = (component as IMiniMapViewComponent);
				_pixelOffset = _minimapViewComponent.GetPixelOffset();
				_halfMapSize = _minimapViewComponent.GetScaledHalfMapSize();
			}
			else if (component is IMapModeComponent)
			{
				_mapModeComponent = (component as IMapModeComponent);
			}
			else if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData += HandleInputData;
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IMiniMapViewComponent)
			{
				_minimapViewComponent = null;
			}
			else if (component is IMapModeComponent)
			{
				_mapModeComponent = null;
			}
			else if (component is IMapInputComponent)
			{
				(component as IMapInputComponent).OnInputData -= HandleInputData;
			}
		}

		public void Tick(float deltaSec)
		{
			if (_closeTimer > 0f)
			{
				_closeTimer -= deltaSec;
			}
		}

		private void HandleInputData(InputCharacterData input)
		{
			OnShowMapInput(input);
		}

		private void OnShowMapInput(InputCharacterData input)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			float num = input.data[15];
			if (!_canPing)
			{
				return;
			}
			if (num > 0f)
			{
				if (!_shown)
				{
					ActivatePingView(activate: true);
					EventManager.get_Instance().PostEvent("GUI_MapPing_Open", 0);
					_shown = true;
				}
				if (!_contextChanged)
				{
					_minimapViewComponent.SetCanPing(canPing: true);
					_mapModeComponent.SwitchMode(MMode.ShowMap);
					cursorMode.PushFreeMode();
					Cursor.SetCursor(_minimapViewComponent.GetPingMouseCursor(), Vector2.get_zero(), 0);
					_contextChanged = true;
				}
				_closeTimer = _minimapViewComponent.GetCloseTime();
			}
			else
			{
				if (_shown && _closeTimer <= 0f)
				{
					ActivatePingView(activate: false);
					EventManager.get_Instance().PostEvent("GUI_MapPing_Close", 0);
					_shown = false;
				}
				if (_contextChanged)
				{
					_minimapViewComponent.SetCanPing(canPing: false);
					_mapModeComponent.SwitchMode(MMode.HideMap);
					cursorMode.PopFreeMode();
					Cursor.SetCursor(_minimapViewComponent.GetDefaultMouseCursor(), Vector2.get_zero(), 0);
					_contextChanged = false;
				}
			}
		}

		private bool CheckIfOutsideMap(Vector3 clickMousePosition)
		{
			Vector3 val = default(Vector3);
			val._002Ector((float)Screen.get_width() - clickMousePosition.x + _pixelOffset.x, clickMousePosition.y - _pixelOffset.y, clickMousePosition.z);
			return (val.x > _halfMapSize * 2f || val.y > _halfMapSize * 2f) ? true : false;
		}

		private void ActivatePingView(bool activate)
		{
			_minimapViewComponent.SetIsPingContextActive(activate);
			if (!_minimapViewComponent.GetIsMinimapZoomed() && _minimapViewComponent.GetIsPingContextActive())
			{
				_minimapViewComponent.ToggleMinimap();
				_previousIsZoomed = false;
			}
			else if (_minimapViewComponent.GetIsPingContextActive())
			{
				_previousIsZoomed = true;
			}
			if (!_previousIsZoomed && !_minimapViewComponent.GetIsPingContextActive())
			{
				_minimapViewComponent.ToggleMinimap();
			}
		}

		private void OnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (isMe)
			{
				_canPing = false;
				if (_contextChanged)
				{
					_minimapViewComponent.SetCanPing(canPing: false);
					_mapModeComponent.SwitchMode(MMode.HideMap);
					cursorMode.PopFreeMode();
					_contextChanged = false;
				}
			}
		}

		private void OnRespawnedIn(SpawnInParametersPlayer spawnInParameter)
		{
			if (spawnInParameter.isMe)
			{
				_canPing = true;
			}
		}

		private void OnGameEnded(bool won)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			_canPing = false;
			if (_contextChanged)
			{
				_minimapViewComponent.SetCanPing(canPing: false);
				_mapModeComponent.SwitchMode(MMode.HideMap);
				cursorMode.PopFreeMode();
				Cursor.SetCursor(_minimapViewComponent.GetDefaultMouseCursor(), Vector2.get_zero(), 0);
				_contextChanged = false;
			}
		}
	}
}
