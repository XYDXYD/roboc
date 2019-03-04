using Fabric;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class MirrorMode : IHandleEditingInput, IInitialize, IInputComponent, IComponent
	{
		private static bool ModeEnabledCache;

		private static int PositionCache;

		private bool _enabled;

		private bool _canEnableMirrorMode = true;

		private bool _initialActivationHasOccured;

		private int _position;

		[Inject]
		public IDispatchWorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		public bool IsEnabled => _enabled;

		public event Action<bool> OnMirrorModeChanged = delegate
		{
		};

		public event Action<int, int> OnMirrorLineMoved = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			worldSwitching.OnWorldJustSwitched += WorldSwitching_OnWorldJustSwitched;
			_initialActivationHasOccured = false;
		}

		private void WorldSwitching_OnWorldJustSwitched(WorldSwitchMode current)
		{
			_canEnableMirrorMode = (current == WorldSwitchMode.BuildMode);
			this.OnMirrorModeChanged(_enabled && _canEnableMirrorMode);
			if (_canEnableMirrorMode && !_initialActivationHasOccured)
			{
				if (ModeEnabledCache)
				{
					SwitchMode(forceOff: false);
				}
				if (PositionCache != 0)
				{
					MoveLine(PositionCache);
				}
				_initialActivationHasOccured = true;
			}
		}

		public void HandleEditingInput(InputEditingData data)
		{
			if (_canEnableMirrorMode && data[EditingInputAxis.MIRROR_MODE] == 1f)
			{
				SwitchMode(forceOff: false);
			}
			if (_enabled)
			{
				if (data[EditingInputAxis.MOVE_MIRROR_LINE] > 0.5f)
				{
					MoveLine(1);
				}
				else if (data[EditingInputAxis.MOVE_MIRROR_LINE] < -0.5f)
				{
					MoveLine(-1);
				}
			}
		}

		public void SwitchMode(bool forceOff)
		{
			if (forceOff && !_enabled)
			{
				ModeEnabledCache = false;
			}
			else if (_canEnableMirrorMode)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
				_enabled = !_enabled;
				this.OnMirrorModeChanged(_enabled && _canEnableMirrorMode);
				ModeEnabledCache = _enabled;
			}
		}

		private void MoveLine(int direction)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonHover));
			_position += direction;
			int position = _position;
			Byte3 @byte = machineMap.GridSize();
			int num = -@byte.x + 1;
			Byte3 byte2 = machineMap.GridSize();
			_position = Mathf.Clamp(position, num, byte2.x - 1);
			CalculateLinePosition(_position, out int centreLineOffset, out int fullLineOffset);
			this.OnMirrorLineMoved(centreLineOffset, fullLineOffset);
			PositionCache = _position;
		}

		public void CurrentLinePosition(out int centreLineOffset, out int fullLineOffset)
		{
			CalculateLinePosition(_position, out centreLineOffset, out fullLineOffset);
		}

		private void CalculateLinePosition(int movementUnits, out int centreLineOffset, out int fullLineOffset)
		{
			fullLineOffset = 0;
			if ((_position & 1) == 1)
			{
				fullLineOffset = ((_position > 0) ? 1 : (-1));
			}
			centreLineOffset = _position / 2;
		}
	}
}
