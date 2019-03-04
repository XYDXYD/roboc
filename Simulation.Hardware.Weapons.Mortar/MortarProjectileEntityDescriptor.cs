using Simulation.Hardware.Weapons.Plasma;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Mortar
{
	internal class MortarProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static MortarProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[3]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<PlasmaProjectileNode>(),
				new EntityViewBuilder<GenericProjectileTrailNode>()
			};
		}

		public MortarProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
