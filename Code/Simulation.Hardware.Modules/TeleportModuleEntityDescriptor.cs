using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TeleportModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[14]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<TeleportModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModuleReadyEffectNode>(),
				new EntityViewBuilder<ModulePowerConsumptionNode>(),
				new EntityViewBuilder<LoadTeleportModuleStatsNode>(),
				new EntityViewBuilder<TeleportModuleEffectsNode>(),
				new EntityViewBuilder<TeleportModuleTeleporterNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public TeleportModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
