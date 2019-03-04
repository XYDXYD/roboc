using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorBladeNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRotorDataComponent rotorDataComponent;

		public IHeightChangeComponent heightChangeComponent;

		public IStrafeComponent strafeComponent;

		public ITurningComponent turningComponent;

		public IDriftComponent driftComponent;

		public ILevelingComponent levelingComponent;

		public ITiltComponent tiltComponent;

		public IForcePointScaleComponent forcePointScaleComponent;

		public ICeilingHeightModifierComponent ceilingHeightModifierComponent;

		public IRotorForceMagnitudeComponent forceMagnitudeComponent;

		public IMaxCarryMassComponent maxCarryMassComponent;

		public IHardwareDisabledComponent disabledComponent;

		public ISpeedModifierComponent speedModifierComponent;

		public RotorBladeNode()
			: this()
		{
		}
	}
}
