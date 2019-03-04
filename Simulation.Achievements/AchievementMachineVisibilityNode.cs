using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.Achievements
{
	internal sealed class AchievementMachineVisibilityNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IMachineVisibilityComponent machineVisibilityComponent;

		public AchievementMachineVisibilityNode()
			: this()
		{
		}
	}
}
