using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class MachineMotorDetectionEntityView : EntityView
	{
		public IInputMotorComponent motorComponent;

		public IMachineFunctionalComponent functionalsComponent;

		public IMachineInputComponent inputComponent;

		public IMachineStunComponent stunComponent;

		public MachineMotorDetectionEntityView()
			: this()
		{
		}
	}
}
