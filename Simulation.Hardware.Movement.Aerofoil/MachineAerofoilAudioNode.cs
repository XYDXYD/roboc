using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class MachineAerofoilAudioNode : EntityView
	{
		public IMachineAerofoilComponent aerofoilComponent;

		public IMachineOwnerComponent ownerComponent;

		public IAudioGameObjectComponent audioGOComponent;

		public IMachineAerofoilAudioComponent audioComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public MachineAerofoilAudioNode()
			: this()
		{
		}
	}
}
