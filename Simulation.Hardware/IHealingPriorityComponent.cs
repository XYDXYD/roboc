namespace Simulation.Hardware
{
	internal interface IHealingPriorityComponent
	{
		int healerId
		{
			get;
			set;
		}

		float priorityExpireTime
		{
			get;
			set;
		}
	}
}
