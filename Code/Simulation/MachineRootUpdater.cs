using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class MachineRootUpdater : IInitialize
	{
		private FasterList<uint> indices = new FasterList<uint>();

		private DestroyedCubesFinder _destroyedCubesFinder;

		private FasterList<InstantiatedCube> _newSeparatedCubes = new FasterList<InstantiatedCube>();

		private Queue<CubeNodeInstance> _cubesToProcess = new Queue<CubeNodeInstance>();

		private FasterList<CubeNodeInstance> _allNodes = new FasterList<CubeNodeInstance>();

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachineCpuDataManager cpuManager
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_destroyedCubesFinder = new DestroyedCubesFinder();
		}

		internal FasterList<InstantiatedCube> GetDisconnectedCubes(TargetType type, int machineId, FasterList<InstantiatedCube> destroyedCubes)
		{
			indices.FastClear();
			bool isRootDestroyed = false;
			FasterList<InstantiatedCube> val = _destroyedCubesFinder.FindSeperatedCubes(destroyedCubes, indices, ref isRootDestroyed);
			if (type != 0)
			{
				return val;
			}
			uint currentCpu = 0u;
			IMachineMap machineMap = machineManager.GetMachineMap(type, machineId);
			if (!isRootDestroyed)
			{
				currentCpu = cpuManager.GetCurrentCpu(machineId);
				UpdateCurrentCPU(ref currentCpu, destroyedCubes, val);
			}
			uint num = currentCpu;
			int num2 = -1;
			for (int i = 0; i < indices.get_Count(); i += 3)
			{
				if (indices.get_Item(i + 2) > num)
				{
					num = indices.get_Item(i + 2);
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				HashSet<InstantiatedCube> remainingCubes = machineMap.GetRemainingCubes();
				_newSeparatedCubes.FastClear();
				if (!isRootDestroyed)
				{
					HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						InstantiatedCube current = enumerator.Current;
						if (!current.cubeNodeInstance.isDestroyed)
						{
							_newSeparatedCubes.Add(current);
							current.cubeNodeInstance.isDestroyed = true;
						}
					}
				}
				for (int j = 0; j < indices.get_Count(); j += 3)
				{
					uint num3 = indices.get_Item(j);
					uint num4 = num3 + indices.get_Item(j + 1);
					if (j != num2)
					{
						for (uint num5 = num3; num5 < num4; num5++)
						{
							_newSeparatedCubes.Add(val.get_Item((int)num5));
						}
						continue;
					}
					for (uint num6 = num3; num6 < num4; num6++)
					{
						val.get_Item((int)num6).cubeNodeInstance.isDestroyed = false;
					}
					if (!isRootDestroyed)
					{
						UpdateGraph(val.get_Item((int)indices.get_Item(j)).cubeNodeInstance);
					}
				}
				return _newSeparatedCubes;
			}
			return val;
		}

		private void UpdateCurrentCPU(ref uint currentCpu, FasterList<InstantiatedCube> destroyedCubes, FasterList<InstantiatedCube> separatedCubes)
		{
			for (int i = 0; i < destroyedCubes.get_Count(); i++)
			{
				currentCpu -= destroyedCubes.get_Item(i).persistentCubeData.cpuRating;
			}
			for (int j = 0; j < separatedCubes.get_Count(); j++)
			{
				currentCpu -= separatedCubes.get_Item(j).persistentCubeData.cpuRating;
			}
		}

		private void UpdateGraph(CubeNodeInstance root)
		{
			root.linkToChair = null;
			root.processed = true;
			_cubesToProcess.Enqueue(root);
			_allNodes.Add(root);
			while (_cubesToProcess.Count > 0)
			{
				CubeNodeInstance cubeNodeInstance = _cubesToProcess.Dequeue();
				FasterList<CubeNodeInstance> neighbours = cubeNodeInstance.GetNeighbours();
				for (int num = neighbours.get_Count() - 1; num >= 0; num--)
				{
					CubeNodeInstance cubeNodeInstance2 = neighbours.get_Item(num);
					if (!cubeNodeInstance2.processed && !cubeNodeInstance2.isDestroyed)
					{
						cubeNodeInstance2.processed = true;
						cubeNodeInstance2.linkToChair = cubeNodeInstance;
						_cubesToProcess.Enqueue(cubeNodeInstance2);
						_allNodes.Add(cubeNodeInstance2);
					}
				}
			}
			for (int num2 = _allNodes.get_Count() - 1; num2 >= 0; num2--)
			{
				CubeNodeInstance cubeNodeInstance3 = _allNodes.get_Item(num2);
				cubeNodeInstance3.processed = false;
			}
			_allNodes.FastClear();
		}
	}
}
