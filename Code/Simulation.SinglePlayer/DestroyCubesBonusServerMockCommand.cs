namespace Simulation.SinglePlayer
{
	internal class DestroyCubesBonusServerMockCommand : AggregateCubesCpuBonusServerMockCommand
	{
		protected override void InitialiseGameStatId()
		{
			base.gameStatId = InGameStatId.DestroyedCubes;
		}
	}
}
