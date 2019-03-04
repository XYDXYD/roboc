using Fabric;
using Svelto.Command;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal class CentreRobotCommand : IInjectableCommand<string>, ICommand
	{
		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal MachineMover machineMover
		{
			private get;
			set;
		}

		[Inject]
		internal BuildHistoryManager buildHistoryManager
		{
			private get;
			set;
		}

		public void Execute()
		{
			MachineModel machineModel = machineMap.BuildMachineLayoutModel();
			machineMap.CalculatOffsetToCentreForModel(machineModel, out int offsetX, out int offsetZ);
			Int3 translation = new Int3(offsetX, 0, offsetZ);
			if (translation != Int3.zero)
			{
				HashSet<InstantiatedCube> allCubes = machineMap.GetAllInstantiatedCubes();
				if (machineMover.IsMoveValid(allCubes, translation))
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.SubMenuSwap));
					Int3 reverseTranslation = new Int3(-1 * translation.x, -1 * translation.y, -1 * translation.z);
					Action action = delegate
					{
						machineMover.MoveCubesToValidCell(allCubes, reverseTranslation);
					};
					buildHistoryManager.StoreUndoBuildAction(action);
					action = delegate
					{
						HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
						machineMover.MoveCubesToValidCell(allInstantiatedCubes, translation);
					};
					action();
				}
				else
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CubeRotate));
				}
			}
		}

		public ICommand Inject(string input)
		{
			return this;
		}
	}
}
