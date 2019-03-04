using Services.Simulation;

internal sealed class PlayerDataDependency
{
	public readonly string PlayerName;

	public readonly string DisplayName;

	public readonly string RobotName;

	public readonly string RobotUniqueId;

	public readonly int TeamId;

	public readonly bool HasPremium;

	public readonly byte[] RobotData;

	public readonly int Cpu;

	public readonly AvatarInfo AvatarInfo;

	public readonly string ClanName;

	public readonly AvatarInfo ClanAvatarInfo;

	public readonly bool AiPlayer;

	public readonly int MasteryLevel;

	public readonly int Tier;

	public readonly string SpawnEffect;

	public readonly string DeathEffect;

	public WeaponOrderSimulation WeaponOrder
	{
		get;
		set;
	}

	public byte[] RobotColourData
	{
		get;
		set;
	}

	public int PlatoonId
	{
		get;
		set;
	}

	public PlayerDataDependency(string playerName, string displayName, string robotName, byte[] cubeMap, uint team, bool hasPremium, string robotUniqueId, int cpu, int masteryLevel, int tier, AvatarInfo avatarInfo, string clanName, AvatarInfo clanAvatarInfo, bool aiPlayer, string spawnEffect, string deathEffect, int[] weaponOrder, byte[] colourMap = null)
	{
		AvatarInfo = avatarInfo;
		ClanName = clanName;
		ClanAvatarInfo = clanAvatarInfo;
		PlayerName = playerName;
		DisplayName = displayName;
		RobotName = robotName;
		RobotData = cubeMap;
		TeamId = (int)team;
		HasPremium = hasPremium;
		WeaponOrder = new WeaponOrderSimulation(weaponOrder);
		RobotColourData = colourMap;
		RobotUniqueId = robotUniqueId;
		Cpu = cpu;
		PlatoonId = 255;
		MasteryLevel = masteryLevel;
		Tier = tier;
		AiPlayer = aiPlayer;
		SpawnEffect = spawnEffect;
		DeathEffect = deathEffect;
	}

	public override string ToString()
	{
		return "PlayerDataDependency: " + PlayerName;
	}
}
