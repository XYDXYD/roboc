using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class HealingGraphTraverser
	{
		private Queue<CubeNodeInstance> _cubesToProcess = new Queue<CubeNodeInstance>();

		private FasterList<CubeNodeInstance> _allProcessedNodes = new FasterList<CubeNodeInstance>();

		public void SelectCubesToHeal(int totalHealingRequired, List<HitCubeInfo> healedCubes, HashSet<InstantiatedCube> curableCubes)
		{
			InstantiatedCube current;
			using (HashSet<InstantiatedCube>.Enumerator enumerator = curableCubes.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return;
				}
				current = enumerator.Current;
			}
			CubeNodeInstance cubeNodeInstance = current.cubeNodeInstance;
			_cubesToProcess.Clear();
			_allProcessedNodes.FastClear();
			cubeNodeInstance.processed = true;
			_cubesToProcess.Enqueue(cubeNodeInstance);
			_allProcessedNodes.Add(cubeNodeInstance);
			CubeNodeInstance cubeNodeInstance2 = null;
			int num = 0;
			while (_cubesToProcess.Count > 0 && num < totalHealingRequired)
			{
				cubeNodeInstance2 = _cubesToProcess.Dequeue();
				int num2 = cubeNodeInstance2.instantiatedCube.totalHealth - cubeNodeInstance2.instantiatedCube.health;
				num2 = Mathf.Min(num2, totalHealingRequired - num);
				if (num2 > 0)
				{
					num += num2;
					HitCubeInfo item = default(HitCubeInfo);
					item.damage = num2;
					item.gridLoc = cubeNodeInstance2.instantiatedCube.gridPos;
					healedCubes.Add(item);
				}
				FasterList<CubeNodeInstance> originalNeighbours = cubeNodeInstance2.GetOriginalNeighbours();
				for (int num3 = originalNeighbours.get_Count() - 1; num3 >= 0; num3--)
				{
					CubeNodeInstance cubeNodeInstance3 = originalNeighbours.get_Item(num3);
					bool flag = cubeNodeInstance3.instantiatedCube.totalHealth == cubeNodeInstance3.instantiatedCube.health;
					if (!cubeNodeInstance3.processed && !flag)
					{
						cubeNodeInstance3.processed = true;
						_cubesToProcess.Enqueue(cubeNodeInstance3);
						_allProcessedNodes.Add(cubeNodeInstance3);
					}
				}
				if (_cubesToProcess.Count == 0)
				{
					foreach (InstantiatedCube curableCube in curableCubes)
					{
						if (!curableCube.cubeNodeInstance.processed)
						{
							cubeNodeInstance = curableCube.cubeNodeInstance;
							cubeNodeInstance.processed = true;
							_cubesToProcess.Enqueue(cubeNodeInstance);
							_allProcessedNodes.Add(cubeNodeInstance);
							break;
						}
					}
				}
			}
			for (int num4 = _allProcessedNodes.get_Count() - 1; num4 >= 0; num4--)
			{
				CubeNodeInstance cubeNodeInstance4 = _allProcessedNodes.get_Item(num4);
				cubeNodeInstance4.processed = false;
			}
		}
	}
}
