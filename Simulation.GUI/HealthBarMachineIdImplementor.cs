using Svelto.ECS;

namespace Simulation.GUI
{
	internal class HealthBarMachineIdImplementor : IHealthBarMachineIdComponent
	{
		public int machineId
		{
			get;
			set;
		}

		public DispatchOnSet<bool> isActive
		{
			get;
			private set;
		}

		public HealthBarMachineIdImplementor(int entityId)
		{
			isActive = new DispatchOnSet<bool>(entityId);
		}
	}
}
