using Svelto.ECS;

namespace EnginesGUI
{
	internal class ScreenConfigPresetsEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ScreenConfigPresetsEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<ScreenConfigurationNode>()
			};
		}

		public ScreenConfigPresetsEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
