using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ShieldModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ShieldModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[11]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<ShieldModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModuleReadyEffectNode>(),
				new EntityViewBuilder<ModulePowerConsumptionNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public ShieldModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
