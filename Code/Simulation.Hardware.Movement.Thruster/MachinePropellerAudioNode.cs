using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class MachinePropellerAudioNode : EntityView
	{
		public IPropellerAudioNamesComponent audioNamesComponent;

		public IPropellerAudioStateComponent audioStateComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public IRigidBodyComponent rbComponent;

		public MachinePropellerAudioNode()
			: this()
		{
		}
	}
}
