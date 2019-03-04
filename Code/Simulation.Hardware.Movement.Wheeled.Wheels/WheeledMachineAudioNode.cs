using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal sealed class WheeledMachineAudioNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMotorAudioStateComponent motorAudioStateComponent;

		public IWheelAudioStateComponent wheelAudioStateComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public IAudioNameComponent audioNameComponent;

		public IMachineFunctionalComponent machineRectifierComponent;

		public WheeledMachineAudioNode()
			: this()
		{
		}
	}
}
