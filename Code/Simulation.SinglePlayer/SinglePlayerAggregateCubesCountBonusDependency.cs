using Simulation.NamedCollections;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal class SinglePlayerAggregateCubesCountBonusDependency : DestroyedHealedCubesBonusDependency
	{
		public SinglePlayerAggregateCubesCountBonusDependency()
			: base(new Dictionary<int, PlayersCubes>())
		{
		}

		public SinglePlayerAggregateCubesCountBonusDependency(byte[] data)
			: base(data)
		{
		}

		public void SetData(Dictionary<int, PlayersCubes> aggregatedCubesDependencyData)
		{
			base.shooterTargetPlayers = aggregatedCubesDependencyData;
		}
	}
}
