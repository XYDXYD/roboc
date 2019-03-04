using Svelto.IoC;

namespace Mothership
{
	internal sealed class NormalBattleAvailability
	{
		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		public bool HasSufficientLevelForRanked(BattleAvailabilityDependancyData availabilityDependancyData)
		{
			return availabilityDependancyData.playerLevel >= availabilityDependancyData.maxLevelForLeague;
		}

		public GameModeAvailabilityState GetBattleAvailability(BattleAvailabilityDependancyData availabilityDependancyData)
		{
			if (availabilityDependancyData.playerLevel < availabilityDependancyData.maxLevelForLeague)
			{
				return GameModeAvailabilityState.PlayerLevelTooLow;
			}
			if (cpuPower.TotalActualCPUCurrentRobot > cpuPower.MaxCpuPower)
			{
				return GameModeAvailabilityState.CPUTooHighLocked;
			}
			return GameModeAvailabilityState.Enabled;
		}
	}
}
