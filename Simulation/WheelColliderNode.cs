using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class WheelColliderNode : EntityView
	{
		public IWheelColliderComponent wheelColliderComponent;

		public IHardwareOwnerComponent hardwareOwnerComponent;

		public WheelColliderNode()
			: this()
		{
		}
	}
}
