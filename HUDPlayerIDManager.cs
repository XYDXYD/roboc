using Simulation;
using Simulation.Hardware.Weapons;
using Simulation.TeamBuff;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class HUDPlayerIDManager : IInitialize, IWaitForFrameworkDestruction
{
	[Inject]
	public DestructionReporter destructionReporter
	{
		private get;
		set;
	}

	[Inject]
	public PlayerTeamsContainer playerTeamsContainer
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
	internal MachineCpuDataManager cpuManager
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerCubesBuffedObserver playerCubesBuffedObserver
	{
		private get;
		set;
	}

	public Texture2D AvatarAtlasTexture
	{
		get;
		private set;
	}

	public IDictionary<string, Rect> AvatarAtlasRects
	{
		get;
		private set;
	}

	public Texture2D ClanAvatarAtlasTexture
	{
		get;
		private set;
	}

	public IDictionary<string, Rect> ClanAvatarAtlasRects
	{
		get;
		private set;
	}

	public event Action<int, uint, uint> OnPlayerHUDIDCreate = delegate
	{
	};

	public event Action<int> OnPlayerHUDIDDestroy = delegate
	{
	};

	public event Action<int, float, bool> OnPlayerHealthChange = delegate
	{
	};

	public event Action<int, bool> OnPlayerHUDIDEnable = delegate
	{
	};

	public event Action<int, bool> OnPlayerBuffed = delegate
	{
	};

	public event Action<int, uint> OnStreakUpdate;

	public event Action<int> OnLeaderUpdate;

	unsafe void IInitialize.OnDependenciesInjected()
	{
		destructionReporter.OnMachineDestroyed += DestroyHUDPlayerID;
		cpuManager.OnMachineCpuInitialized += HandleOnMachineCpuInitialized;
		cpuManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
		playerCubesBuffedObserver.AddAction(new ObserverAction<PlayerCubesBuffedDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void ActivatePlayerHUDWidget(int owner, bool activate)
	{
		int activeMachine = machinesContainer.GetActiveMachine(TargetType.Player, owner);
		this.OnPlayerHUDIDEnable(activeMachine, activate);
	}

	internal void InjectMultiplayerAvatars(Texture2D avatarAtlasTexture, IDictionary<string, Rect> avatarAtlasRects, Texture2D clanAvatarAtlas, IDictionary<string, Rect> clanAvatarAtlasRects)
	{
		AvatarAtlasTexture = avatarAtlasTexture;
		AvatarAtlasRects = avatarAtlasRects;
		ClanAvatarAtlasTexture = clanAvatarAtlas;
		ClanAvatarAtlasRects = clanAvatarAtlasRects;
	}

	private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int hitmachineId, float percent)
	{
		int playerFromMachineId = machinesContainer.GetPlayerFromMachineId(TargetType.Player, hitmachineId);
		if (!playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
		{
			this.OnPlayerHealthChange(hitmachineId, percent, playerTeamsContainer.IsMe(shooterType, shooterId));
		}
	}

	private void HandleOnMachineCpuInitialized(int machineId, uint health)
	{
		int playerFromMachineId = machinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
		if (!playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
		{
			this.OnPlayerHUDIDCreate(machineId, health, health);
		}
	}

	private void DestroyHUDPlayerID(int playerId, int machineId, bool isMe)
	{
		if (!isMe)
		{
			this.OnPlayerHUDIDDestroy(machineId);
		}
	}

	private void ShowPlayerBuffed(ref PlayerCubesBuffedDependency dependency)
	{
		this.OnPlayerBuffed(dependency.machineId, (double)dependency.buffAmount > 1.0);
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		destructionReporter.OnMachineDestroyed -= DestroyHUDPlayerID;
		cpuManager.OnMachineCpuInitialized -= HandleOnMachineCpuInitialized;
		cpuManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
		playerCubesBuffedObserver.RemoveAction(new ObserverAction<PlayerCubesBuffedDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void StreakUpdate(int playerId, uint streak)
	{
		if (this.OnStreakUpdate != null)
		{
			this.OnStreakUpdate(playerId, streak);
		}
	}

	public void LeaderUpdate(int playerId)
	{
		if (this.OnLeaderUpdate != null)
		{
			this.OnLeaderUpdate(playerId);
		}
	}
}
