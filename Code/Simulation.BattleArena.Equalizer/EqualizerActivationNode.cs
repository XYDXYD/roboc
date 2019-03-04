using Svelto.ECS;

namespace Simulation.BattleArena.Equalizer
{
	internal sealed class EqualizerActivationNode : EntityView
	{
		public IRigidbodyComponent rbComponent;

		public IMachineMapComponent machineMapComponent;

		public IOwnerComponent ownerComponent;

		public IAnimatorComponent animatorComponent;

		public IVisualTeamComponent visualTeamComponent;

		public IOwnerTeamComponent teamComponent;

		public EqualizerActivationNode()
			: this()
		{
		}
	}
}
