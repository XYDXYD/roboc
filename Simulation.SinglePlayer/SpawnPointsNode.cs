using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	internal class SpawnPointsNode : EntityView
	{
		public ISpawnPointsComponent spawnPoints;

		public SpawnPointsNode()
			: this()
		{
		}
	}
}
