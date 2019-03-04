using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class GameEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static GameEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<GameMovementAllowedEntityView>()
			};
		}

		public GameEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
