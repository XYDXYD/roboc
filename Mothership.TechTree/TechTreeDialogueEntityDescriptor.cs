using Svelto.ECS;

namespace Mothership.TechTree
{
	internal sealed class TechTreeDialogueEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _entityViewsToBuild;

		static TechTreeDialogueEntityDescriptor()
		{
			_entityViewsToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<TechTreeDialogueEntityView>()
			};
		}

		public TechTreeDialogueEntityDescriptor()
			: this(_entityViewsToBuild)
		{
		}
	}
}
