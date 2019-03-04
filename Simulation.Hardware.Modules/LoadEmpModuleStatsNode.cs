using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadEmpModuleStatsNode : EntityView
	{
		public IEmpModuleCountdownComponent countdownComponent;

		public IEmpModuleStunDurationComponent stunDurationComponent;

		public IEmpModuleStunRadiusComponent stunRadiusComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public LoadEmpModuleStatsNode()
			: this()
		{
		}
	}
}
