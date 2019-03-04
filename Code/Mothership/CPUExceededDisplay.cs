using Svelto.DataStructures;
using Svelto.IoC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Mothership
{
	internal sealed class CPUExceededDisplay : IInitialize
	{
		private uint _cpuUsed;

		private HashSet<InstantiatedCube> _overCPUCubes = new HashSet<InstantiatedCube>();

		private List<InstantiatedCube> cubes = new List<InstantiatedCube>();

		[Inject]
		public ICPUPower cpuPower
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
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		public MachineColorUpdater colorUpdater
		{
			private get;
			set;
		}

		[Inject]
		public MachineEditorGraphUpdater graphUpdater
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			cpuPower.RegisterOnCPULoadChanged(OnCpuUsedChanged);
			graphUpdater.OnGraphInitialized += TryMarkExcessCpu;
		}

		internal void UpdateOnCubeDeleted(InstantiatedCube cube)
		{
			if (_overCPUCubes.Count > 0 && !_overCPUCubes.Remove(cube))
			{
				MarkExcessCpu();
			}
		}

		private void OnCpuUsedChanged(uint cpuUsed)
		{
			_cpuUsed = cpuUsed;
		}

		private void TryMarkExcessCpu(FasterList<InstantiatedCube> cubes)
		{
			ClearOverCPUCubes();
			if (_cpuUsed > cpuPower.MaxMegabotCpuPower)
			{
				MarkExcessCpu();
			}
		}

		public bool AreCubesOverCPU()
		{
			return _overCPUCubes.Count > 0;
		}

		public ReadOnlyCollection<InstantiatedCube> GetOverCPUCubes()
		{
			cubes.Clear();
			cubes.AddRange(_overCPUCubes);
			return cubes.AsReadOnly();
		}

		public void ClearOverCPUCubes()
		{
			_overCPUCubes.Clear();
		}

		private void MarkExcessCpu()
		{
			Queue<InstantiatedCube> queue = new Queue<InstantiatedCube>();
			HashSet<InstantiatedCube> hashSet = new HashSet<InstantiatedCube>();
			InstantiatedCube machineRoot = GetMachineRoot();
			if (machineRoot == null)
			{
				return;
			}
			uint num = 0u;
			queue.Enqueue(machineRoot);
			while (queue.Count > 0 && cpuPower.MaxMegabotCpuPower - num != 0)
			{
				InstantiatedCube instantiatedCube = queue.Dequeue();
				CubeTypeID cubeType = instantiatedCube.persistentCubeData.cubeType;
				uint cubeCPURating = cubeList.GetCubeCPURating(cubeType);
				if (num + cubeCPURating > cpuPower.MaxMegabotCpuPower)
				{
					continue;
				}
				hashSet.Add(instantiatedCube);
				num += cubeCPURating;
				CubeNodeInstance cubeNodeInstance = instantiatedCube.cubeNodeInstance;
				FasterList<CubeNodeInstance> neighbours = cubeNodeInstance.GetNeighbours();
				for (int i = 0; i < neighbours.get_Count(); i++)
				{
					InstantiatedCube instantiatedCube2 = neighbours.get_Item(i).instantiatedCube;
					if (!hashSet.Contains(instantiatedCube2) && !queue.Contains(instantiatedCube2))
					{
						queue.Enqueue(instantiatedCube2);
					}
				}
			}
			HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			foreach (InstantiatedCube item in allInstantiatedCubes)
			{
				GameObject cubeAt = machineMap.GetCubeAt(item.gridPos);
				if (cubeAt != null)
				{
					bool flag = !hashSet.Contains(item);
					bool flag2 = _overCPUCubes.Contains(item);
					if (flag && !flag2)
					{
						_overCPUCubes.Add(item);
						colorUpdater.MarkAsValid(cubeAt, isValid: false);
					}
					else if (!flag && flag2)
					{
						_overCPUCubes.Remove(item);
						colorUpdater.MarkAsValid(cubeAt, isValid: true);
					}
				}
			}
		}

		private void ClearExcessCpu()
		{
			foreach (InstantiatedCube overCPUCube in _overCPUCubes)
			{
				GameObject cubeAt = machineMap.GetCubeAt(overCPUCube.gridPos);
				if (cubeAt != null)
				{
					colorUpdater.MarkAsValid(cubeAt, isValid: true);
				}
			}
			_overCPUCubes.Clear();
		}

		private InstantiatedCube GetMachineRoot()
		{
			using (HashSet<InstantiatedCube>.Enumerator enumerator = machineMap.GetAllInstantiatedCubes().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				return null;
			}
		}
	}
}
