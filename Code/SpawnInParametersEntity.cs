using Simulation.Hardware.Weapons;

internal sealed class SpawnInParametersEntity
{
	public readonly int EntityID;

	public readonly int TeamId;

	public readonly PreloadedMachine PreloadedMachine;

	public readonly TargetType Type;

	public readonly bool IsOnMyTeam;

	public readonly BattleArenaExtraData BaseData;

	public SpawnInParametersEntity(int entityID, int teamId, bool isOnMyTeam, TargetType type, PreloadedMachine preloadedMachine, BattleArenaExtraData baseData)
	{
		EntityID = entityID;
		TeamId = teamId;
		IsOnMyTeam = isOnMyTeam;
		Type = type;
		PreloadedMachine = preloadedMachine;
		BaseData = baseData;
	}
}
