using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Laser
{
	internal class StraightProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static StraightProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LaserProjectileNode>(),
				new EntityViewBuilder<ProjectileNode>(),
				new EntityViewBuilder<GenericProjectileTrailNode>()
			};
		}

		public StraightProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
