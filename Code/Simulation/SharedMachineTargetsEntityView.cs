using BehaviorDesigner.Runtime;
using System;

namespace Simulation
{
	[Serializable]
	public class SharedMachineTargetsEntityView : SharedVariable<MachineTargetsEntityView>
	{
		public static implicit operator SharedMachineTargetsEntityView(MachineTargetsEntityView value)
		{
			SharedMachineTargetsEntityView sharedMachineTargetsEntityView = new SharedMachineTargetsEntityView();
			sharedMachineTargetsEntityView.set_Value(value);
			return sharedMachineTargetsEntityView;
		}
	}
}
