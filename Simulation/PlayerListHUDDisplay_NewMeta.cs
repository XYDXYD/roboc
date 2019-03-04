using Battle;
using Simulation.Hardware.Weapons;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class PlayerListHUDDisplay_NewMeta : MonoBehaviour, IInitialize
	{
		public bool enemyTeam;

		public GameObject widgetPrefab;

		private UIGrid _grid;

		private Dictionary<int, PlayerListHUDWidget_NewMeta> _widgets = new Dictionary<int, PlayerListHUDWidget_NewMeta>();

		[Inject]
		internal MachineSpawnDispatcher machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
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
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineCpuDataManager cpuDataManager
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
		internal GameStartDispatcher startDispatcher
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
		internal PlayerNamesContainer PlayerNameContainer
		{
			private get;
			set;
		}

		public PlayerListHUDDisplay_NewMeta()
			: this()
		{
		}

		private void Awake()
		{
			_grid = this.GetComponent<UIGrid>();
		}

		void IInitialize.OnDependenciesInjected()
		{
			RegisterEvents();
		}

		private void OnDestroy()
		{
			DeregisterEvents();
		}

		private void RegisterEvents()
		{
			machineSpawnDispatcher.OnPlayerRegistered += OnPlayerRegistered;
			machineSpawnDispatcher.OnPlayerSpawnedIn += OnPlayerSpawnedIn;
			machineSpawnDispatcher.OnPlayerRespawnedIn += OnPlayerSpawnedIn;
			machineSpawnDispatcher.OnLocalPlayerReadyToRespawn += HandleOnLocalPlayerReadyToRespawn;
			machineSpawnDispatcher.OnPlayerRespawnScheduled += OnRespawnScheduled;
			cpuDataManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
			destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
			this.get_gameObject().SetActive(false);
			startDispatcher.Register(ShowWidgets);
		}

		private void DeregisterEvents()
		{
			machineSpawnDispatcher.OnPlayerRegistered -= OnPlayerRegistered;
			machineSpawnDispatcher.OnPlayerSpawnedIn -= OnPlayerSpawnedIn;
			machineSpawnDispatcher.OnPlayerRespawnedIn -= OnPlayerSpawnedIn;
			machineSpawnDispatcher.OnLocalPlayerReadyToRespawn -= HandleOnLocalPlayerReadyToRespawn;
			machineSpawnDispatcher.OnPlayerRespawnScheduled -= OnRespawnScheduled;
			cpuDataManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
			destructionReporter.OnMachineDestroyed -= OnMachineDestroyed;
			startDispatcher.Unregister(ShowWidgets);
		}

		private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int machineId, float percent)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (_widgets.TryGetValue(playerFromMachineId, out PlayerListHUDWidget_NewMeta value))
			{
				value.SetCurrentHealthPercent(percent);
			}
		}

		private void HandleOnLocalPlayerReadyToRespawn(int playerId)
		{
			if (_widgets.TryGetValue(playerId, out PlayerListHUDWidget_NewMeta value))
			{
				value.ResetTimer();
			}
		}

		private void ShowWidgets()
		{
			this.get_gameObject().SetActive(true);
		}

		private void OnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			if (spawnInParameters.isOnMyTeam != enemyTeam && !_widgets.ContainsKey(spawnInParameters.playerId))
			{
				string playerName = PlayerNameContainer.GetPlayerName(spawnInParameters.playerId);
				string displayName = PlayerNameContainer.GetDisplayName(spawnInParameters.playerId);
				GameObject val = gameObjectFactory.Build(widgetPrefab);
				val.set_name(displayName);
				_grid.AddChild(val.get_transform());
				val.get_transform().set_localScale(Vector3.get_one());
				val.SetActive(true);
				_grid.Reposition();
				PlayerListHUDWidget_NewMeta component = val.GetComponent<PlayerListHUDWidget_NewMeta>();
				bool isMe = spawnInParameters.isMe;
				bool isInMyPlatoon = BattlePlayers.MyPlatoonId != 255 && BattlePlayers.MyPlatoonId == BattlePlayers.GetPlatoonId(playerName);
				component.Initialise(displayName, isMe, enemyTeam, isInMyPlatoon);
				TaskRunner.get_Instance().Run(SetAvatars(playerName, component));
				_widgets.Add(spawnInParameters.playerId, component);
			}
		}

		private IEnumerator SetAvatars(string playerName, PlayerListHUDWidget_NewMeta widget)
		{
			while (PlayerListHudPresenter.AvatarAtlasTexture == null)
			{
				yield return null;
			}
			widget.AvatarTexture.set_mainTexture(PlayerListHudPresenter.AvatarAtlasTexture);
			widget.AvatarTexture.set_uvRect(PlayerListHudPresenter.AvatarAtlasRects[playerName]);
			if (PlayerListHudPresenter.ClanAvatarAtlasRects.TryGetValue(playerName, out Rect clanavatarRect))
			{
				widget.ClanAvatarTexture.set_mainTexture(PlayerListHudPresenter.ClanAvatarAtlasTexture);
				widget.ClanAvatarTexture.set_uvRect(clanavatarRect);
				widget.ClanAvatarTexture.get_gameObject().SetActive(true);
			}
		}

		private void OnPlayerSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (_widgets.TryGetValue(spawnInParameters.playerId, out PlayerListHUDWidget_NewMeta value))
			{
				value.Respawn();
			}
		}

		private void OnRespawnScheduled(int playerId, int timeSeconds)
		{
			if (_widgets.TryGetValue(playerId, out PlayerListHUDWidget_NewMeta value))
			{
				value.RespawnScheduled(timeSeconds);
			}
		}

		private void OnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			MarkAsDead(playerId);
		}

		private void MarkAsDead(int playerId)
		{
			if (_widgets.TryGetValue(playerId, out PlayerListHUDWidget_NewMeta value))
			{
				value.MarkAsDead();
			}
		}
	}
}
