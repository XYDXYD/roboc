using Svelto.DataStructures;

namespace Simulation
{
	internal class MachineTargetsImplementor : IMachineTargetInfoComponent, IMachineTargetsComponent
	{
		private readonly TargetInfo _targetInfo = new TargetInfo();

		private readonly FasterList<MachineTargetsEntityView> _allyTargets = new FasterList<MachineTargetsEntityView>();

		private readonly FasterList<MachineTargetsEntityView> _enemyTargets = new FasterList<MachineTargetsEntityView>();

		private readonly FasterList<MachineTargetsEntityView> _visibleTargets = new FasterList<MachineTargetsEntityView>();

		public TargetInfo targetInfo => _targetInfo;

		public FasterList<MachineTargetsEntityView> allyTargets => _allyTargets;

		public FasterList<MachineTargetsEntityView> enemyTargets => _enemyTargets;

		public FasterList<MachineTargetsEntityView> visibleTargets => _visibleTargets;
	}
}
