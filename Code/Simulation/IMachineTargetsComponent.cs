using Svelto.DataStructures;

namespace Simulation
{
	public interface IMachineTargetsComponent
	{
		FasterList<MachineTargetsEntityView> allyTargets
		{
			get;
		}

		FasterList<MachineTargetsEntityView> enemyTargets
		{
			get;
		}

		FasterList<MachineTargetsEntityView> visibleTargets
		{
			get;
		}
	}
}
