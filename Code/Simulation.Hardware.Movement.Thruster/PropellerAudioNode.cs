using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class PropellerAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IPartLevelComponent levelComponent;

		public ISpinningBladesComponent spinningComponent;

		public IHardwareDisabledComponent disabledComponent;

		public PropellerAudioNode()
			: this()
		{
		}
	}
}
