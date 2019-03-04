using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorBladeGraphicsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRotorDataComponent rotorDataComponent;

		public IRotorGraphicsComponent rotorGraphicsComponent;

		public ITiltPivotTransformComponent tiltPivotTransformComponent;

		public ISpinningPivotTransformComponent spinningPivotTransformComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IVisibilityComponent visibilityComponent;

		public RotorBladeGraphicsNode()
			: this()
		{
		}
	}
}
