namespace Simulation.SinglePlayer
{
	internal class ProtectTeamMateCubesBonusServerMockCommand : AggregateCubesCpuBonusServerMockCommand
	{
		protected override void InitialiseGameStatId()
		{
			base.gameStatId = InGameStatId.DestroyedCubesInProtection;
		}
	}
}
