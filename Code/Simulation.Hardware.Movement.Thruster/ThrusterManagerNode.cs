using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class ThrusterManagerNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IFacingDirectionComponent facingComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IVerticalOrientationThrusterCountComponent verticalCountComponent;

		public ITypeComponent typeComponent;

		public ThrusterManagerNode()
			: this()
		{
		}
	}
}
