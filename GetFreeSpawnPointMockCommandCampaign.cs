internal sealed class GetFreeSpawnPointMockCommandCampaign : GetFreeSpawnPointServerMockCommand
{
	public override bool UseOnlyTeamBasedSpawnPoints => false;

	public override SpawnPoints.SpawnPointsType SpawnType => SpawnPoints.SpawnPointsType.PitModeStartLocations;
}
