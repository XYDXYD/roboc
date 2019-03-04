using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	internal class SpawnPointsDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static SpawnPointsDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<SpawnPointsNode>()
			};
		}

		public SpawnPointsDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
