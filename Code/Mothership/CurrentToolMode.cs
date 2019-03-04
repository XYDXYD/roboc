using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal sealed class CurrentToolMode : IInitialize, IHandleEditingInput, IInputComponent, IComponent
	{
		public enum SwitchingLockTypes
		{
			Painting,
			TutorialNode,
			Count
		}

		public enum ToolMode
		{
			Build,
			Paint,
			NoTool,
			Count
		}

		private bool _canChangeModeDueToCursorMode;

		private bool[] _modesBlocked;

		private bool[] _locksObtained;

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		public ToolMode currentBuildTool
		{
			get;
			private set;
		}

		public event Action<ToolMode> OnToolModeChanged = delegate
		{
		};

		public event Action<ToolMode> OnWeaponBlocked = delegate
		{
		};

		public event Action<ToolMode> OnWeaponUnblocked = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			_modesBlocked = new bool[3];
			_locksObtained = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				_locksObtained[i] = false;
			}
			for (int j = 0; j < 3; j++)
			{
				_modesBlocked[j] = false;
			}
			currentBuildTool = ToolMode.Build;
			cursorMode.OnSwitch += HandleOnSwitch;
		}

		public void ObtainSwitchingLock(SwitchingLockTypes switchingLockType)
		{
			_locksObtained[(int)switchingLockType] = true;
		}

		public void ReleaseSwitchingLock(SwitchingLockTypes switchingLockType)
		{
			_locksObtained[(int)switchingLockType] = false;
		}

		public void BlockMode(ToolMode modeToBlock)
		{
			_modesBlocked[(int)modeToBlock] = true;
			this.OnWeaponBlocked(modeToBlock);
		}

		public void UnblockMode(ToolMode modeToBlock)
		{
			_modesBlocked[(int)modeToBlock] = false;
			this.OnWeaponUnblocked(modeToBlock);
		}

		public void ForceImmediateModeChange(ToolMode toolMode)
		{
			currentBuildTool = toolMode;
			this.OnToolModeChanged(currentBuildTool);
		}

		public void TryChangeToolMode(ToolMode toolMode)
		{
			for (int i = 0; i < 2; i++)
			{
				if (_locksObtained[i])
				{
					return;
				}
			}
			if (toolMode != currentBuildTool && !_modesBlocked[(int)toolMode])
			{
				currentBuildTool = toolMode;
				this.OnToolModeChanged(currentBuildTool);
			}
		}

		void IHandleEditingInput.HandleEditingInput(InputEditingData data)
		{
			if (_canChangeModeDueToCursorMode)
			{
				if (data[EditingInputAxis.CUBE_MANIPULATOR] > 0.5f && currentBuildTool != 0)
				{
					TryChangeToolMode(ToolMode.Build);
				}
				else if (data[EditingInputAxis.PAINT_APPLICATOR] > 0.5f && currentBuildTool != ToolMode.Paint)
				{
					TryChangeToolMode(ToolMode.Paint);
				}
			}
		}

		private void HandleOnSwitch(Mode mode)
		{
			_canChangeModeDueToCursorMode = (mode == Mode.Lock);
		}
	}
}
