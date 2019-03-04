using Simulation.Hardware.Weapons.Laser;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ChaingunProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LaserProjectileNode>(),
				new EntityViewBuilder<ProjectileNode>(),
				new EntityViewBuilder<RecycleProjectileOnResetNode>()
			};
		}

		public ChaingunProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
