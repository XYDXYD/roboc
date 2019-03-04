using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal sealed class SkiMachineAudioNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public ISkiAudioStateComponent skiAudioStateComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public IMachineFunctionalComponent machineRectifierComponent;

		public SkiMachineAudioNode()
			: this()
		{
		}
	}
}
