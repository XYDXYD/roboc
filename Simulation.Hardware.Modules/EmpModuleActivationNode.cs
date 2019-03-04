using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IModuleActivationComponent activationComponent;

		public IModuleRangeComponent rangeComponent;

		public IEmpModuleStunRadiusComponent stunRadiusComponent;

		public IEmpModuleStunDurationComponent durationComponent;

		public IEmpModuleCountdownComponent countdownComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public EmpModuleActivationNode()
			: this()
		{
		}
	}
}
