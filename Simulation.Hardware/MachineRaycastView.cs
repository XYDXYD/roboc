using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineRaycastView : EntityView
	{
		public IMachineStunComponent stunComponent;

		public IMachineInputComponent inputComponent;

		public IWeaponRaycastComponent raycastComponent;

		public MachineRaycastView()
			: this()
		{
		}
	}
}
