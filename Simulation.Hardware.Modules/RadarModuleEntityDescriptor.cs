using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class RadarModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RadarModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[13]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModuleReadyEffectNode>(),
				new EntityViewBuilder<ModulePowerConsumptionNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<RadarModuleEntityView>(),
				new EntityViewBuilder<LoadRadarModuleStatsNode>(),
				new EntityViewBuilder<RadarModuleFeedbackNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public RadarModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
