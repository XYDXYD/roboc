using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RemoteMachineRotorNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IRotorInputComponent inputComponent;

		public IMachineInputComponent inputWrapperComponent;

		public ICacheUpdateComponent cacheUpdateComponent;

		public IRotorPowerValueComponent powerValueComponent;

		public IAudioLiftingLoweringComponent audioLiftingLoweringComponent;

		public IMachineStunComponent machineStunComponent;

		public RemoteMachineRotorNode()
			: this()
		{
		}
	}
}
