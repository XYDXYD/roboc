using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic.Balloon
{
	internal class BalloonNode : EntityView
	{
		public IBalloonComponent balloonComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public IHardwareOwnerComponent hardwareOwnerComponent;

		public ITransformComponent transformComponent;

		public BalloonNode()
			: this()
		{
		}
	}
}
