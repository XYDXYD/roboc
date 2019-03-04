using Simulation.Hardware.Weapons.RocketLauncher;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Nano
{
	internal class NanoProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static NanoProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<HealingProjectileImpactNode>(),
				new EntityViewBuilder<HomingProjectileNode>(),
				new EntityViewBuilder<RocketLauncherProjectileTrailNode>()
			};
		}

		public NanoProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
