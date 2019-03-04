using Svelto.ECS;

namespace EnginesGUI
{
	internal class RetargetableSpriteEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RetargetableSpriteEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<RetargetableSpriteNode>()
			};
		}

		public RetargetableSpriteEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
