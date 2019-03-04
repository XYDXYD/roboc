using Svelto.ECS;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverBladeNode : EntityView
	{
		public IHoverInfoComponent info;

		public IMaxCarryMassComponent massComponent;

		public IValidMovementComponent validComponent;

		public IHardwareDisabledComponent disabledComponent;

		public HoverBladeNode()
			: this()
		{
		}
	}
}
