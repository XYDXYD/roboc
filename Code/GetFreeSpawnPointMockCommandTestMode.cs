internal class GetFreeSpawnPointMockCommandTestMode : GetFreeSpawnPointServerMockCommand
{
	public override bool UseOnlyTeamBasedSpawnPoints => false;

	public override SpawnPoints.SpawnPointsType SpawnType => SpawnPoints.SpawnPointsType.TestModeEnemy;
}
