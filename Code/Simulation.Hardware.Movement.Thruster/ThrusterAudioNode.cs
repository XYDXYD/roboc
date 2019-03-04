using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class ThrusterAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IInputReceivedComponent inputReceivedComponent;

		public IPartLevelComponent levelComponent;

		public IThrusterForceAppliedComponent forceAppliedComponent;

		public ThrusterAudioNode()
			: this()
		{
		}
	}
}
