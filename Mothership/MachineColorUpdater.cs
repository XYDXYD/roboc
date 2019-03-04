using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineColorUpdater : IInitialize, IWaitForFrameworkDestruction
	{
		private readonly FasterList<InstantiatedCube> _batchedCubes = new FasterList<InstantiatedCube>();

		[Inject]
		internal MachineEditorGraphUpdater graphUpdater
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
		internal MachineEditorBatcher batcher
		{
			private get;
			set;
		}

		public event Action OnCubePainted = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			graphUpdater.OnGraphInitialized += HandleOnGraphInitialized;
			graphUpdater.OnCubesConnected += MarkAsValid;
			graphUpdater.OnCubesDisconnected += MarkAsInvalid;
			EnableShaderKeywords();
		}

		private void EnableShaderKeywords()
		{
			Shader.DisableKeyword("RED_OFF");
			Shader.EnableKeyword("RED_ON");
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			graphUpdater.OnCubesConnected -= MarkAsValid;
			graphUpdater.OnCubesDisconnected -= MarkAsInvalid;
			graphUpdater.OnGraphInitialized -= HandleOnGraphInitialized;
		}

		private void HandleOnGraphInitialized(FasterList<InstantiatedCube> disconnectedCubes)
		{
			MarkAsInvalid(disconnectedCubes);
		}

		internal void UpdateOnCubeDeleted(InstantiatedCube cube)
		{
		}

		internal void UpdateOnCubePlaced(InstantiatedCube cube)
		{
			GameObject cubeAt = machineMap.GetCubeAt(cube.gridPos);
			cubeAt.GetComponent<CubeColorUpdater>().SetColor(cube.paletteColor);
		}

		private void MarkAsInvalid(FasterList<InstantiatedCube> cubes)
		{
			MarkAsValid(cubes, isValid: false);
		}

		private void MarkAsValid(FasterList<InstantiatedCube> cubes)
		{
			MarkAsValid(cubes, isValid: true);
		}

		internal void MarkAsValid(GameObject go, bool isValid)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Int3 gridLoc = GridScaleUtility.WorldToGrid(go.get_transform().get_localPosition(), TargetType.Player);
			MachineCell cellAt = machineMap.GetCellAt(gridLoc);
			if (cellAt == null)
			{
				return;
			}
			InstantiatedCube info = cellAt.info;
			if ((isValid && info.isRed == 1) || (!isValid && info.isRed == 0))
			{
				if (!batcher.IsBatched(info))
				{
					go.GetComponent<CubeColorUpdater>().isValid = isValid;
				}
				else
				{
					batcher.UpdateInvalidColor(info);
				}
			}
			if (isValid)
			{
				if (info.isRed > 0)
				{
					info.isRed--;
				}
			}
			else
			{
				info.isRed++;
			}
		}

		private void MarkAsValid(FasterList<InstantiatedCube> cubes, bool isValid)
		{
			FasterList<InstantiatedCube> val = new FasterList<InstantiatedCube>();
			for (int i = 0; i < cubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = cubes.get_Item(i);
				if ((isValid && instantiatedCube.isRed == 1) || (!isValid && instantiatedCube.isRed == 0))
				{
					if (!batcher.IsBatched(instantiatedCube))
					{
						GameObject cubeAt = machineMap.GetCubeAt(cubes.get_Item(i).gridPos);
						cubeAt.GetComponent<CubeColorUpdater>().isValid = isValid;
					}
					else
					{
						val.Add(instantiatedCube);
					}
				}
				if (isValid)
				{
					if (instantiatedCube.isRed > 0)
					{
						instantiatedCube.isRed--;
					}
				}
				else
				{
					instantiatedCube.isRed++;
				}
			}
			if (val.get_Count() > 0)
			{
				batcher.UpdateInvalidColor(val);
			}
		}

		public void PaintCube(InstantiatedCube cube)
		{
			if (cube.paletteIndex != cube.previousPaletteIndex)
			{
				this.OnCubePainted();
				if (!batcher.IsBatched(cube))
				{
					GameObject cubeAt = machineMap.GetCubeAt(cube.gridPos);
					cubeAt.GetComponent<CubeColorUpdater>().SetColor(cube.paletteColor);
				}
				else
				{
					batcher.UpdateMainColor(cube);
				}
			}
		}

		internal void PaintGroupOfCubes(FasterList<InstantiatedCube> cubes)
		{
			_batchedCubes.FastClear();
			for (int i = 0; i < cubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = cubes.get_Item(i);
				if (!batcher.IsBatched(instantiatedCube))
				{
					GameObject cubeAt = machineMap.GetCubeAt(instantiatedCube.gridPos);
					cubeAt.GetComponent<CubeColorUpdater>().SetColor(instantiatedCube.paletteColor);
				}
				else
				{
					_batchedCubes.Add(instantiatedCube);
				}
			}
			if (_batchedCubes.get_Count() > 0)
			{
				batcher.UpdateMainColor(_batchedCubes);
			}
		}
	}
}
