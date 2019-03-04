using Fabric;
using Mothership;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using System.Collections.Generic;

internal sealed class RobotNudgeManager : IInitialize, IHandleEditingInput, IInputComponent, IComponent
{
	private bool _canNudgeRobot;

	[Inject]
	public ICursorMode cursorMode
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

	[Inject]
	public BuildHistoryManager buildHistoryManager
	{
		private get;
		set;
	}

	[Inject]
	public MachineMover machineMover
	{
		private get;
		set;
	}

	public void OnDependenciesInjected()
	{
		cursorMode.OnSwitch += HandleOnCursorModeChange;
	}

	public void IWaitForFrameworkDestruction()
	{
		if (cursorMode != null)
		{
			cursorMode.OnSwitch -= HandleOnCursorModeChange;
		}
	}

	public void HandleEditingInput(InputEditingData inputEditingData)
	{
		if (!_canNudgeRobot)
		{
			return;
		}
		Int3 translation = new Int3(inputEditingData[EditingInputAxis.NUDGE_ROBOT_X], inputEditingData[EditingInputAxis.NUDGE_ROBOT_Y], inputEditingData[EditingInputAxis.NUDGE_ROBOT_Z]);
		if (translation != Int3.zero)
		{
			HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			if (machineMover.IsMoveValid(allInstantiatedCubes, translation))
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.SubMenuSwap));
				Int3 reverseTranslation = new Int3(-1 * translation.x, -1 * translation.y, -1 * translation.z);
				Action action = delegate
				{
					HashSet<InstantiatedCube> allInstantiatedCubes3 = machineMap.GetAllInstantiatedCubes();
					machineMover.MoveCubesToValidCell(allInstantiatedCubes3, reverseTranslation);
				};
				buildHistoryManager.StoreUndoBuildAction(action);
				action = delegate
				{
					HashSet<InstantiatedCube> allInstantiatedCubes2 = machineMap.GetAllInstantiatedCubes();
					machineMover.MoveCubesToValidCell(allInstantiatedCubes2, translation);
				};
				action();
			}
			else
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubeRotate));
			}
		}
	}

	void IInitialize.OnDependenciesInjected()
	{
		cursorMode.OnSwitch += HandleOnCursorModeChange;
	}

	private void HandleOnCursorModeChange(Mode mode)
	{
		_canNudgeRobot = (mode == Mode.Lock);
	}
}
