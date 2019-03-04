using Simulation.Achievements;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EmpModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[11]
			{
				new EntityViewBuilder<ModuleSelectionNode>(),
				new EntityViewBuilder<ModuleActivationNode>(),
				new EntityViewBuilder<EmpModuleActivationNode>(),
				new EntityViewBuilder<ModuleGUIEntityView>(),
				new EntityViewBuilder<LoadModuleStatsNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<ModulePowerConsumptionNode>(),
				new EntityViewBuilder<LoadEmpModuleStatsNode>(),
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<ModuleCooldownNode>(),
				new EntityViewBuilder<AchievementModuleActivationNode>()
			};
		}

		public EmpModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
