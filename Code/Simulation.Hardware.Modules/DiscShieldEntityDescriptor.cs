using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class DiscShieldEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static DiscShieldEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[5]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<DiscShieldManagingNode>(),
				new EntityViewBuilder<DiscShieldEffectsNode>(),
				new EntityViewBuilder<DiscShieldAudioNode>(),
				new EntityViewBuilder<DiscShieldSettingsNode>()
			};
		}

		public DiscShieldEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
