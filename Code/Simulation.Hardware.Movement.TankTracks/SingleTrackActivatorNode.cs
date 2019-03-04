using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class SingleTrackActivatorNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IRegularCollidersParentComponent regularCollidersParentComponent;

		public ISupportCollidersParentComponent supportCollidersParentComponent;

		public SingleTrackActivatorNode()
			: this()
		{
		}
	}
}
