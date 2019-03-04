using Svelto.ECS;

namespace Simulation.Hardware.Movement.InsectLegs
{
	internal sealed class InsectLegView : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IInsectLegComponent insectLegComponent;

		public InsectLegView()
			: this()
		{
		}
	}
}
