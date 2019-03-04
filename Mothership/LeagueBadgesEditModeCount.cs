using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class LeagueBadgesEditModeCount : IWaitForFrameworkInitialization
	{
		private uint numLeagueBadgesUsed;

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		public ICubeList cubeList
		{
			private get;
			set;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			machineMap.OnAddCubeAt += OnAddCube;
			machineMap.OnRemoveCubeAt += OnRemoveCube;
			AddStartingCubes();
		}

		public bool DoesContainLeagueBadge()
		{
			return numLeagueBadgesUsed != 0;
		}

		private void OnAddCube(Byte3 pos, MachineCell cell)
		{
			OnAddCube(cell.info);
		}

		private void OnAddCube(InstantiatedCube info)
		{
			if (info.persistentCubeData.specialCubeKind == SpecialCubesKind.LeagueBadge)
			{
				numLeagueBadgesUsed++;
			}
		}

		private void OnRemoveCube(Byte3 pos, MachineCell cell)
		{
			if (cell.info.persistentCubeData.specialCubeKind == SpecialCubesKind.LeagueBadge)
			{
				numLeagueBadgesUsed--;
			}
		}

		private void AddStartingCubes()
		{
			numLeagueBadgesUsed = 0u;
			ICollection<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			foreach (InstantiatedCube item in allInstantiatedCubes)
			{
				OnAddCube(item);
			}
		}
	}
}
