using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class DamageVignetteEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _entityViewsToBuild;

		static DamageVignetteEntityDescriptor()
		{
			_entityViewsToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<DamageVignetteEntityView>()
			};
		}

		public DamageVignetteEntityDescriptor()
			: this(_entityViewsToBuild)
		{
		}
	}
}
