using Mothership;
using Mothership.GUI;
using Svelto.Context;
using Svelto.ECS;

namespace Tiers
{
	internal class RobotRankingEngine : IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		private readonly IMachineMap _machineMap;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RobotRankingEngine(IMachineMap machineMap)
		{
			_machineMap = machineMap;
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			_machineMap.OnAddCubeAt += DispatchAddedCubeRankingAndCPU;
			_machineMap.OnRemoveCubeAt += DispatchRemovedCubeRankingAndCPU;
		}

		public void OnFrameworkDestroyed()
		{
			_machineMap.OnAddCubeAt -= DispatchAddedCubeRankingAndCPU;
			_machineMap.OnRemoveCubeAt -= DispatchRemovedCubeRankingAndCPU;
		}

		private void DispatchAddedCubeRankingAndCPU(Byte3 gridLoc, MachineCell machineCell)
		{
			InstantiatedCube info = machineCell.info;
			DispatchCubeRankingAndCPU(info.persistentCubeData, 1);
		}

		private void DispatchRemovedCubeRankingAndCPU(Byte3 gridLoc, MachineCell machineCell)
		{
			InstantiatedCube info = machineCell.info;
			DispatchCubeRankingAndCPU(info.persistentCubeData, -1);
		}

		private void DispatchCubeRankingAndCPU(PersistentCubeData persistentCubeData, int sign)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			int num = (int)persistentCubeData.cpuRating * sign;
			int totalCosmeticCPU = 0;
			int ranking = persistentCubeData.cubeRanking * sign;
			if (persistentCubeData.itemType == ItemType.Cosmetic)
			{
				totalCosmeticCPU = num;
			}
			RobotRankingWidgetsEntityView robotRankingWidgetsEntityView = entityViewsDB.QueryEntityViews<RobotRankingWidgetsEntityView>().get_Item(0);
			robotRankingWidgetsEntityView.RobotRankingComponent.CubeRanking.set_value(new RankingAndCPU(ranking, num, totalCosmeticCPU));
		}
	}
}
