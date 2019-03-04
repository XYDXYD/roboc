using Simulation.Hardware;

namespace Simulation
{
	internal class MachineHealingPriorityImplementor : IHealingPriorityComponent
	{
		public int healerId
		{
			get;
			set;
		}

		public float priorityExpireTime
		{
			get;
			set;
		}

		public MachineHealingPriorityImplementor()
		{
			healerId = -1;
		}
	}
}
