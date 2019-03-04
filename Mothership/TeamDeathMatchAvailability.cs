using Svelto.IoC;

namespace Mothership
{
	internal sealed class TeamDeathMatchAvailability
	{
		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		public GameModeAvailabilityState GetBattleAvailability(BattleAvailabilityDependancyData availabilityDependancyData)
		{
			if (cpuPower.TotalActualCPUCurrentRobot > cpuPower.MaxCpuPower)
			{
				return GameModeAvailabilityState.CPUTooHighLocked;
			}
			return GameModeAvailabilityState.Enabled;
		}
	}
}
