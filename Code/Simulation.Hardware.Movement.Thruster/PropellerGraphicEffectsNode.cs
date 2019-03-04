using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class PropellerGraphicEffectsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IInputReceivedComponent inputReceivedComponent;

		public IPartLevelComponent levelComponent;

		public IThrusterForceAppliedComponent forceAppliedComponent;

		public ISpinningBladesComponent spinningComponent;

		public IVisibilityComponent visibilityComponent;

		public ITransformComponent transformComponent;

		public IAccelerationDelayComponent accComponent;

		public PropellerGraphicEffectsNode()
			: this()
		{
		}
	}
}
