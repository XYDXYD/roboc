using Svelto.IoC;
using System;

internal class MachineSpawnDispatcher
{
	[Inject]
	public LivePlayersContainer livePlayersContainer
	{
		private get;
		set;
	}

	[Inject]
	public NetworkMachineManager networkMachineManager
	{
		private get;
		set;
	}

	[Inject]
	public MachineRootContainer machineRootcontainer
	{
		private get;
		set;
	}

	public event Action<SpawnInParametersPlayer> OnPlayerRegistered = delegate
	{
	};

	public event Action<SpawnInParametersPlayer> OnPlayerSpawnedIn = delegate
	{
	};

	public event Action<SpawnOutParameters> OnPlayerSpawnedOut = delegate
	{
	};

	public event Action<SpawnInParametersPlayer> OnPlayerRespawnedIn = delegate
	{
	};

	public event Action<UnregisterParametersPlayer> OnPlayerUnregistered = delegate
	{
	};

	public event Action<int, int> OnPlayerRespawnScheduled = delegate
	{
	};

	public event Action<int> OnLocalPlayerReadyToRespawn = delegate
	{
	};

	public event Action<SpawnInParametersEntity> OnEntitySpawnedIn = delegate
	{
	};

	public event Action<SpawnInParametersEntity> OnEntityRespawnedIn = delegate
	{
	};

	public void PlayerRegistered(SpawnInParametersPlayer spawnInParameters)
	{
		this.OnPlayerRegistered(spawnInParameters);
	}

	public void PlayerSpawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		this.OnPlayerSpawnedIn(spawnInParameters);
	}

	public void PlayerSpawnedOut(int player)
	{
		this.OnPlayerSpawnedOut(new SpawnOutParameters(player, machineRootcontainer, networkMachineManager, livePlayersContainer));
	}

	public void PlayerRespawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		this.OnPlayerRespawnedIn(spawnInParameters);
	}

	public void PlayerUnregistered(UnregisterParametersPlayer unregisterParameters)
	{
		this.OnPlayerUnregistered(unregisterParameters);
	}

	internal void ScheduleRespawn(int playerId, int time)
	{
		this.OnPlayerRespawnScheduled(playerId, time);
	}

	internal void EntitySpawnedIn(SpawnInParametersEntity spawnInParameters)
	{
		this.OnEntitySpawnedIn(spawnInParameters);
	}

	internal void EntityRespawnedIn(SpawnInParametersEntity spawnInParameters)
	{
		this.OnEntityRespawnedIn(spawnInParameters);
	}

	internal void LocalPlayerReadyToRespawn(int playerId)
	{
		this.OnLocalPlayerReadyToRespawn(playerId);
	}
}
