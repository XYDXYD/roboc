using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineWeaponOrderView : EntityView
	{
		public IWeaponOrderComponent orderComponent;

		public IMachineOwnerComponent ownerComponent;

		public MachineWeaponOrderView()
			: this()
		{
		}
	}
}
