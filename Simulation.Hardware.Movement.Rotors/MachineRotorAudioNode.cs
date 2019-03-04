using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class MachineRotorAudioNode : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public IAudioGameObjectComponent audioGameObjectComponent;

		public IAudioLiftingLoweringComponent audioLiftingLoweringComponent;

		public IRotorPowerValueComponent powerValueComponent;

		public IPlayingAudioComponent playingAudioComponent;

		public IMachineRotorAudioLevelComponent machineAudioLevelComponent;

		public MachineRotorAudioNode()
			: this()
		{
		}
	}
}
