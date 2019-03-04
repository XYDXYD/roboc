using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineMover : IInitialize
	{
		private FasterList<InstantiatedCube> validCubesToMove = new FasterList<InstantiatedCube>();

		private FasterList<GameObject> gameObjects = new FasterList<GameObject>();

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		public IMachineBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		public CubeBoundsCache cubeBoundsCache
		{
			private get;
			set;
		}

		public event Action<Int3> OnMachineMoved = delegate
		{
		};

		public event Action<HashSet<InstantiatedCube>> cubesMoved = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
		}

		public bool CanMoveMachineWhithNoCubesDestroyed(HashSet<InstantiatedCube> cubelist, Int3 displacement)
		{
			foreach (InstantiatedCube item in cubelist)
			{
				Int3 worldPos = (Int3)item.gridPos + displacement;
				if (!machineMap.IsPosValid(worldPos))
				{
					return false;
				}
			}
			return true;
		}

		public void MoveCubesToValidCellOrDestroy(HashSet<InstantiatedCube> cubelist, Int3 displacement)
		{
			validCubesToMove.FastClear();
			List<InstantiatedCube> list = new List<InstantiatedCube>();
			foreach (InstantiatedCube item in cubelist)
			{
				Int3 worldPos = (Int3)item.gridPos + displacement;
				if (!machineMap.IsPosValid(worldPos))
				{
					list.Add(item);
				}
				else
				{
					validCubesToMove.Add(item);
				}
			}
			foreach (InstantiatedCube item2 in list)
			{
				machineBuilder.RemoveCube(item2);
			}
			list = null;
			ActuallyMoveCubes(displacement);
		}

		public void MoveCubesToValidCell(HashSet<InstantiatedCube> cubelist, Int3 displacement)
		{
			validCubesToMove = new FasterList<InstantiatedCube>((ICollection<InstantiatedCube>)cubelist);
			ActuallyMoveCubes(displacement);
		}

		private void ActuallyMoveCubes(Int3 displacement)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			gameObjects.FastClear();
			Vector3 val = GridScaleUtility.WorldScale(displacement, TargetType.Player);
			for (int i = 0; i < validCubesToMove.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = validCubesToMove.get_Item(i);
				GameObject cubeAt = machineMap.GetCubeAt(instantiatedCube.gridPos);
				machineMap.RemoveCellFromMachineMap(instantiatedCube.gridPos);
				gameObjects.Add(cubeAt);
			}
			for (int j = 0; j < validCubesToMove.get_Count(); j++)
			{
				InstantiatedCube instantiatedCube = validCubesToMove.get_Item(j);
				GameObject val2 = gameObjects.get_Item(j);
				instantiatedCube.gridPos += displacement;
				Transform transform = val2.get_transform();
				transform.set_position(transform.get_position() + val);
				machineMap.SilentlyAddCellToMachineMap(instantiatedCube.gridPos, instantiatedCube, val2);
			}
			this.OnMachineMoved(displacement);
			this.cubesMoved(machineMap.GetAllInstantiatedCubes());
		}

		public bool IsMoveValid(HashSet<InstantiatedCube> cubelist, Int3 translation)
		{
			if (cubelist.Count == 0)
			{
				return false;
			}
			foreach (InstantiatedCube item in cubelist)
			{
				Int3 @int = (Int3)item.gridPos;
				Int3 int2 = @int + translation;
				if (!machineMap.IsPosValid(int2))
				{
					return false;
				}
				CubeExtentBounds cubeBounds = cubeBoundsCache.GetCubeBounds(item);
				if (!machineMap.IsPosValid(int2 + cubeBounds.lowerBound) && !WasOriginalPosInvalid(@int, cubeBounds.lowerBound))
				{
					return false;
				}
				if (!machineMap.IsPosValid(int2 + cubeBounds.upperBound) && !WasOriginalPosInvalid(@int, cubeBounds.upperBound))
				{
					return false;
				}
			}
			return true;
		}

		private bool WasOriginalPosInvalid(Int3 gridPos, Int3 bound)
		{
			return !machineMap.IsPosValid(gridPos + bound);
		}
	}
}
