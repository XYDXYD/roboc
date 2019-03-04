namespace Simulation.SinglePlayer
{
	internal class HealCubesBonusServerMockCommand : AggregateCubesCpuBonusServerMockCommand
	{
		protected override void InitialiseGameStatId()
		{
			base.gameStatId = InGameStatId.HealCubes;
		}
	}
}
