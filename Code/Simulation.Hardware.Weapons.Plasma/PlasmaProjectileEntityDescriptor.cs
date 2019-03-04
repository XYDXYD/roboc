using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal class PlasmaProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PlasmaProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[3]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<PlasmaProjectileNode>(),
				new EntityViewBuilder<GenericProjectileTrailNode>()
			};
		}

		public PlasmaProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
