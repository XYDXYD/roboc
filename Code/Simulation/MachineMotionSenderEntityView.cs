using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal class MachineMotionSenderEntityView : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IWeaponRaycastComponent weaponRaycastComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IAliveStateComponent aliveStateComponent;

		public MachineMotionSenderEntityView()
			: this()
		{
		}
	}
}
