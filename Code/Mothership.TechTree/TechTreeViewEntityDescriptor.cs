using Svelto.ECS;

namespace Mothership.TechTree
{
	internal sealed class TechTreeViewEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _entityViewsToBuild;

		static TechTreeViewEntityDescriptor()
		{
			_entityViewsToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[3]
			{
				new EntityViewBuilder<TechTreeViewEntityView>(),
				new EntityViewBuilder<TechTreeViewScrollableEntityView>(),
				new EntityViewBuilder<TechTreeViewZoomableEntityView>()
			};
		}

		public TechTreeViewEntityDescriptor()
			: this(_entityViewsToBuild)
		{
		}
	}
}
