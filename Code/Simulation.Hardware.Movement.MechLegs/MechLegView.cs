using Svelto.ECS;

namespace Simulation.Hardware.Movement.MechLegs
{
	internal sealed class MechLegView : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IMechLegComponent mechLegComponent;

		public MechLegView()
			: this()
		{
		}
	}
}
