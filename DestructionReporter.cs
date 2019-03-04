using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using Utility;

internal sealed class DestructionReporter : IInitialize, IWaitForFrameworkDestruction
{
	private HashSet<int> _destroyedMachines = new HashSet<int>();

	[Inject]
	public MachineRootContainer rootContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineSpawnDispatcher spawnDispatcher
	{
		private get;
		set;
	}

	[Inject]
	public PlayerMachinesContainer machinesContainer
	{
		private get;
		set;
	}

	[Inject]
	public LivePlayersContainer livePlayersContainer
	{
		private get;
		set;
	}

	public event Action<int> OnPlayerSelfDestructs = delegate
	{
	};

	public event Action<int, int> OnMachineKilled = delegate
	{
	};

	public event Action<int, int, bool> OnMachineDestroyed = delegate
	{
	};

	public event Action<DestructionData> OnPlayerDamageApplied = delegate
	{
	};

	public event Action<DestructionData> OnProtoniumDamageApplied = delegate
	{
	};

	public event Action<DestructionData> OnPlayerDamageAppliedEffects = delegate
	{
	};

	public event Action<int> OnMachineDestroyedEffects = delegate
	{
	};

	public void OnDependenciesInjected()
	{
		spawnDispatcher.OnPlayerRespawnedIn += HandleonRespawnedIn;
		spawnDispatcher.OnPlayerUnregistered += HandleOnPlayerUnregistered;
	}

	public void OnFrameworkDestroyed()
	{
		spawnDispatcher.OnPlayerRespawnedIn -= HandleonRespawnedIn;
		spawnDispatcher.OnPlayerUnregistered -= HandleOnPlayerUnregistered;
	}

	public void PostCubeDestroyed(ref DestructionData data)
	{
		if (data.targetType == TargetType.Player)
		{
			this.OnPlayerDamageApplied(data);
			if (data.isDestroyed)
			{
				if (!_destroyedMachines.Contains(data.hitMachineId))
				{
					RegisterKilled(data.hitPlayerId, data.shooterId, data.targetType);
					BroadcastDeath(data.targetIsMe, data.hitMachineId, data.hitPlayerId, data.shooterId);
					_destroyedMachines.Add(data.hitMachineId);
				}
				else
				{
					Console.LogWarning("Destroying machine twice");
				}
			}
		}
		else
		{
			this.OnProtoniumDamageApplied(data);
		}
	}

	public void PostCubeDestroyedEffects(ref DestructionData data)
	{
		if (data.targetType == TargetType.Player)
		{
			this.OnPlayerDamageAppliedEffects(data);
			if (data.isDestroyed)
			{
				this.OnMachineDestroyedEffects(data.hitPlayerId);
			}
		}
	}

	public void PlayerSelfDestructed(int playerId, int machineId, bool isMe)
	{
		if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId))
		{
			livePlayersContainer.MarkAsDead(TargetType.Player, playerId);
			this.OnMachineDestroyed(playerId, machineId, isMe);
			this.OnPlayerSelfDestructs(playerId);
			if (rootContainer.IsMachineRegistered(TargetType.Player, machineId))
			{
				this.OnMachineDestroyedEffects(playerId);
			}
		}
	}

	private void HandleOnPlayerUnregistered(UnregisterParametersPlayer unregisterParameters)
	{
		_destroyedMachines.Remove(unregisterParameters.playerId);
	}

	private void HandleonRespawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		_destroyedMachines.Remove(spawnInParameters.machineId);
	}

	private void BroadcastDeath(bool isMe, int machineId, int ownerId, int shooterId)
	{
		this.OnMachineDestroyed(ownerId, machineId, isMe);
		this.OnMachineKilled(ownerId, shooterId);
	}

	private void RegisterKilled(int ownerId, int shooterId, TargetType targetType)
	{
		livePlayersContainer.MarkAsDead(targetType, ownerId);
	}
}
