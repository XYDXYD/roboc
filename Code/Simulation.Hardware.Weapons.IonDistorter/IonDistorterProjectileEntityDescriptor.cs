using Svelto.ECS;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal class IonDistorterProjectileEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static IonDistorterProjectileEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[2]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<IonDistorterProjectileNode>()
			};
		}

		public IonDistorterProjectileEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
