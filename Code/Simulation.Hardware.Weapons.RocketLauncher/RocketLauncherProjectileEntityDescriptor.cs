using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal class RocketLauncherProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RocketLauncherProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<HomingProjectileNode>(),
				new EntityViewBuilder<RocketLauncherProjectileTrailNode>(),
				new EntityViewBuilder<RocketLauncherImpactNode>()
			};
		}

		public RocketLauncherProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
