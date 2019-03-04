using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorBladeAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IRotorAudioLevelComponent rotorAudioLevelComponent;

		public IVisibilityComponent visibilityComponent;

		public RotorBladeAudioNode()
			: this()
		{
		}
	}
}
