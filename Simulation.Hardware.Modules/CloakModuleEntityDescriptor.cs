using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal class CloakModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static CloakModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[9]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<CloakModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public CloakModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
