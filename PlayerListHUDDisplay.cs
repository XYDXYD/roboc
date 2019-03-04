using Battle;
using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class PlayerListHUDDisplay : MonoBehaviour, IInitialize, IHudElement
{
	private sealed class PlayerKillData
	{
		public int playerId;

		public bool isAiMachine;

		public readonly string name;

		public readonly string displayName;

		public bool isDead;

		public readonly bool isMe;

		public int kills;

		public readonly bool isInMyPlatoon;

		public PlayerKillData(int _playerId, string _name, string _displayName, bool _isMe, bool _isInMyPlatoon, bool _isAiMachine)
		{
			kills = 0;
			isDead = true;
			playerId = _playerId;
			name = _name;
			displayName = _displayName;
			isMe = _isMe;
			isInMyPlatoon = _isInMyPlatoon;
			isAiMachine = _isAiMachine;
		}
	}

	private class PlayerListHudComparer : IComparer<PlayerKillData>
	{
		public int Compare(PlayerKillData a, PlayerKillData b)
		{
			if (a.kills > b.kills)
			{
				return -1;
			}
			if (a.kills < b.kills)
			{
				return 1;
			}
			if (a.isDead && !b.isDead)
			{
				return -1;
			}
			if (!a.isDead && b.isDead)
			{
				return 1;
			}
			if (a.isMe)
			{
				return -1;
			}
			if (b.isMe)
			{
				return 1;
			}
			return 0;
		}
	}

	public bool myTeam = true;

	public GameObject listHUDWidget;

	public GameObject topBar;

	private readonly int _maxPanelsPerTeam = 20;

	private readonly FasterList<PlayerKillData> sortedPlayerKills = new FasterList<PlayerKillData>();

	private readonly List<PlayerListHUDWidget> playerListElements = new List<PlayerListHUDWidget>();

	private readonly Dictionary<int, PlayerKillData> playersByKills = new Dictionary<int, PlayerKillData>();

	private IComparer<PlayerKillData> _comparer = new PlayerListHudComparer();

	[Inject]
	internal MachineSpawnDispatcher machineSpawnDispatcher
	{
		private get;
		set;
	}

	[Inject]
	internal DestructionReporter destructionReporter
	{
		private get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IDispatchWorldSwitching worldSwitch
	{
		private get;
		set;
	}

	[Inject]
	internal LobbyGameStartPresenter lobbyPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerTeamsContainer playerTeamsContainer
	{
		private get;
		set;
	}

	[Inject]
	internal IBattleEventStreamManager battleEventStreamManager
	{
		private get;
		set;
	}

	[Inject]
	internal BattlePlayers BattlePlayers
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerListHudPresenter PlayerListHudPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IHudStyleController battleHudStyleController
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerNamesContainer playerNamesContainer
	{
		private get;
		set;
	}

	public PlayerListHUDDisplay()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		RegisterEvents();
		PlayerListHUDWidget component = listHUDWidget.GetComponent<PlayerListHUDWidget>();
		InitElements();
	}

	private IEnumerator Start()
	{
		battleHudStyleController.AddHud(this);
		while (PlayerListHudPresenter.AvatarAtlasTexture == null)
		{
			yield return null;
		}
		SetAvatarAtlases();
	}

	public void SetStyle(HudStyle style)
	{
		if (style == HudStyle.HideAllButChat)
		{
			this.get_gameObject().SetActive(false);
		}
	}

	public void SetAvatarAtlases()
	{
		foreach (PlayerListHUDWidget playerListElement in playerListElements)
		{
			playerListElement.AvatarTexture.set_mainTexture(PlayerListHudPresenter.AvatarAtlasTexture);
			playerListElement.ClanAvatarTexture.set_mainTexture(PlayerListHudPresenter.ClanAvatarAtlasTexture);
		}
	}

	private void InitElements()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _maxPanelsPerTeam; i++)
		{
			GameObject val = gameObjectFactory.Build(listHUDWidget);
			val.get_transform().set_parent(this.get_transform());
			val.get_transform().set_localScale(Vector3.get_one());
			PlayerListHUDWidget component = val.GetComponent<PlayerListHUDWidget>();
			playerListElements.Add(component);
		}
		topBar.SetActive(false);
	}

	private void OnDestroy()
	{
		UnRegisterEvents();
		battleHudStyleController.RemoveHud(this);
	}

	private void RegisterEvents()
	{
		machineSpawnDispatcher.OnPlayerRegistered += OnPlayerRegistered;
		machineSpawnDispatcher.OnPlayerSpawnedIn += OnPlayerSpawnIn;
		machineSpawnDispatcher.OnPlayerRespawnedIn += OnPlayerRespawnedIn;
		battleEventStreamManager.OnPlayerWasKilledBy += OnMachineKilled;
		destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
		LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
		lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyPresenter.OnInitialLobbyGuiClose, new Action(RefreshList));
	}

	private void UnRegisterEvents()
	{
		if (this.lobbyPresenter != null)
		{
			LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
			lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Remove(lobbyPresenter.OnInitialLobbyGuiClose, new Action(RefreshList));
		}
		if (machineSpawnDispatcher != null)
		{
			machineSpawnDispatcher.OnPlayerSpawnedIn -= OnPlayerSpawnIn;
		}
		battleEventStreamManager.OnPlayerWasKilledBy -= OnMachineKilled;
		if (destructionReporter != null)
		{
			destructionReporter.OnMachineDestroyed -= OnMachineDestroyed;
		}
	}

	private void OnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isOnMyTeam == myTeam)
		{
			bool isMe = spawnInParameters.isMe;
			bool isInMyPlatoon = BattlePlayers.MyPlatoonId != 255 && BattlePlayers.MyPlatoonId == BattlePlayers.GetPlatoonId(playerNamesContainer.GetPlayerName(spawnInParameters.playerId));
			PlayerKillData playerKillData = new PlayerKillData(spawnInParameters.playerId, playerNamesContainer.GetPlayerName(spawnInParameters.playerId), playerNamesContainer.GetDisplayName(spawnInParameters.playerId), isMe, isInMyPlatoon, spawnInParameters.isAIMachine);
			playerKillData.isDead = true;
			playersByKills.Add(spawnInParameters.playerId, playerKillData);
			RefreshList();
		}
	}

	private void OnPlayerSpawnIn(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isOnMyTeam == myTeam)
		{
			if (playersByKills.TryGetValue(spawnInParameters.playerId, out PlayerKillData value))
			{
				value.isDead = false;
			}
			RefreshList();
		}
	}

	private void OnMachineKilled(int playerId, int shooterId)
	{
		if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, shooterId) == myTeam)
		{
			if (playersByKills.ContainsKey(shooterId))
			{
				IncrementKills(shooterId);
			}
			RefreshList();
		}
	}

	private void OnMachineDestroyed(int playerId, int machineId, bool isMe)
	{
		if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId) == myTeam)
		{
			MarkAsDead(playerId);
			RefreshList();
		}
	}

	private void OnPlayerRespawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isOnMyTeam == myTeam)
		{
			if (playersByKills.ContainsKey(spawnInParameters.playerId))
			{
				playersByKills[spawnInParameters.playerId].isDead = false;
			}
			RefreshList();
		}
	}

	private void IncrementKills(int playerId)
	{
		playersByKills[playerId].kills++;
	}

	private void MarkAsDead(int playerId)
	{
		if (playersByKills.ContainsKey(playerId))
		{
			playersByKills[playerId].isDead = true;
		}
	}

	private void RefreshList()
	{
		if (lobbyPresenter.hasBeenClosed)
		{
			topBar.SetActive(true);
			sortedPlayerKills.FastClear();
			sortedPlayerKills.AddRange((ICollection<PlayerKillData>)playersByKills.Values);
			sortedPlayerKills.Sort(_comparer);
			int i;
			for (i = 0; i < sortedPlayerKills.get_Count(); i++)
			{
				PlayerKillData player = sortedPlayerKills.get_Item(i);
				AddPlayerToListAtIndex(player, i);
			}
			for (; i < _maxPanelsPerTeam; i++)
			{
				playerListElements[i].get_gameObject().SetActive(false);
			}
		}
		UITable component = this.GetComponent<UITable>();
		if (component != null)
		{
			component.set_repositionNow(true);
		}
	}

	private void AddPlayerToListAtIndex(PlayerKillData player, int index)
	{
		playerListElements[index].get_gameObject().SetActive(true);
		playerListElements[index].SetPlayerKills(player.kills.ToString(), player.displayName);
		playerListElements[index].SetColour(player.isMe, player.isDead, player.isInMyPlatoon);
		TaskRunner.get_Instance().Run(AssignAvatars(player, index));
	}

	private IEnumerator AssignAvatars(PlayerKillData player, int index)
	{
		while (PlayerListHudPresenter.AvatarAtlasRects == null)
		{
			yield return null;
		}
		playerListElements[index].AvatarTexture.set_uvRect(PlayerListHudPresenter.AvatarAtlasRects[player.name]);
		if (PlayerListHudPresenter.ClanAvatarAtlasRects.TryGetValue(player.name, out Rect clanAvatarRect))
		{
			playerListElements[index].ClanAvatarTexture.set_uvRect(clanAvatarRect);
			playerListElements[index].ClanAvatarTexture.get_gameObject().SetActive(true);
		}
		else
		{
			playerListElements[index].ClanAvatarTexture.get_gameObject().SetActive(false);
		}
	}
}
