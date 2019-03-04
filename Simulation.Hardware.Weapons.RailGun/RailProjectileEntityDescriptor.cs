using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal class RailProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RailProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<RailProjectileNode>(),
				new EntityViewBuilder<ProjectileNode>(),
				new EntityViewBuilder<RailProjectileTrailNode>()
			};
		}

		public RailProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
