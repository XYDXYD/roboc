using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class TopSpeedNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent partDisabledComponent;

		public IMaxSpeedComponent speedComponent;

		public ISpeedModifierComponent speedModifier;

		public ICPUComponent cpuComponent;

		public IValidMovementComponent validMovComponent;

		public IMaxSpeedStatsComponent statsComponent;

		public TopSpeedNode()
			: this()
		{
		}
	}
}
