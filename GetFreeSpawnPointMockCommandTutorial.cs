internal class GetFreeSpawnPointMockCommandTutorial : GetFreeSpawnPointServerMockCommand
{
	public override bool UseOnlyTeamBasedSpawnPoints => false;

	public override SpawnPoints.SpawnPointsType SpawnType => SpawnPoints.SpawnPointsType.TestModeEnemy;
}
