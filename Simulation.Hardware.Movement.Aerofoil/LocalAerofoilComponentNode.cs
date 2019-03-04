using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class LocalAerofoilComponentNode : EntityView
	{
		public IAerofoilComponent aerofoilComponent;

		public IMachineStunComponent machineStunComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public ISpeedModifierComponent speedModifierComponent;

		public IMaxCarryMassComponent carryMassComponent;

		public LocalAerofoilComponentNode()
			: this()
		{
		}
	}
}
