using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal sealed class SkiAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IGroundedComponent groundedComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public ISpeedComponent speedComponent;

		public ISteeringComponent steeringComponent;

		public SkiAudioNode()
			: this()
		{
		}
	}
}
