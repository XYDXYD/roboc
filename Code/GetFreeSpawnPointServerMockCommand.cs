using Battle;
using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal class GetFreeSpawnPointServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private PlayerIdDependency _dependency;

	[Inject]
	public ISpawnPointManager spawnPointManager
	{
		private get;
		set;
	}

	[Inject]
	public INetworkEventManagerServer networkEventManagerServer
	{
		get;
		set;
	}

	[Inject]
	public BattlePlayers battlePlayers
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerNamesContainer playerNamesContainer
	{
		get;
		set;
	}

	public virtual SpawnPoints.SpawnPointsType SpawnType => SpawnPoints.SpawnPointsType.AIStartLocations;

	public virtual bool UseOnlyTeamBasedSpawnPoints => true;

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (PlayerIdDependency)dependency;
		return this;
	}

	public void Execute()
	{
		GetFreeSpawnPoint();
	}

	protected virtual void SendEventToPlayer(int senderId, SpawnPointDependency spawnPointDependency)
	{
		networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeSpawnPoint, senderId, spawnPointDependency);
	}

	private void GetFreeSpawnPoint()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		SpawnPoints.SpawnPointsType spawnPointType = SpawnType;
		int owner = _dependency.owner;
		if (UseOnlyTeamBasedSpawnPoints)
		{
			spawnPointType = ((GetPlayersTeamFromSenderID(owner) != 0) ? SpawnPoints.SpawnPointsType.Team1Start : SpawnPoints.SpawnPointsType.Team0Start);
		}
		SpawningPoint nextFreeSpawnPoint = spawnPointManager.GetNextFreeSpawnPoint(spawnPointType, owner);
		SpawnPointDependency spawnPointDependency = new SpawnPointDependency(nextFreeSpawnPoint.get_transform().get_position(), nextFreeSpawnPoint.get_transform().get_rotation(), owner);
		SendEventToPlayer(_dependency.senderId, spawnPointDependency);
	}

	private int GetPlayersTeamFromSenderID(int id)
	{
		string playerName = string.Empty;
		if (playerNamesContainer.TryGetPlayerName(id, out playerName))
		{
			return (int)battlePlayers.GetTeamId(playerName);
		}
		return 0;
	}
}
