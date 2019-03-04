using Svelto.ECS;

namespace Mothership.TechTree
{
	internal sealed class TechTreeItemEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TechTreeItemEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<TechTreeItemEntityView>(),
				new EntityViewBuilder<TechTreeItemDispatchableEntityView>(),
				new EntityViewBuilder<TechTreeItemInfoEntityView>(),
				new EntityViewBuilder<TechTreeUnlockableEntityView>()
			};
		}

		public TechTreeItemEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
