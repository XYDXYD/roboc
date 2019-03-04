using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class PowerModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PowerModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[12]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModuleReadyEffectNode>(),
				new EntityViewBuilder<ModulePowerConsumptionNode>(),
				new EntityViewBuilder<PowerModuleActivationNode>(),
				new EntityViewBuilder<PowerModuleEffectsNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public PowerModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
