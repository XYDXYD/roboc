using Svelto.ECS;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal class AeroflakProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static AeroflakProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<AeroflakProjectileNode>(),
				new EntityViewBuilder<GenericProjectileTrailNode>(),
				new EntityViewBuilder<StackDamageBonusNode>()
			};
		}

		public AeroflakProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
