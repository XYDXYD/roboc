using Battle;
using Simulation;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class MinimapPresenter : IInitialize, IHudElement, IWaitForFrameworkDestruction, IMinimapPresenter
{
	protected class MinimapPlayerInfo
	{
		public int playerId;

		public bool isVisible;

		public bool isSpotted;

		public Rigidbody rigidbody;
	}

	private const int UPDATE_INTERVAL = 10;

	protected MinimapView _view;

	protected List<int> _activePlayers = new List<int>();

	protected Dictionary<int, MinimapPlayerInfo> _machinesInfo = new Dictionary<int, MinimapPlayerInfo>();

	protected ITaskRoutine _task;

	private Vector3 _bottomRight;

	private Vector3 _topLeft;

	private Texture _minimapTexture;

	private MinimapPlayerInfo _playerMachineInfo;

	private SimulationCamera _camera;

	private bool _dead;

	[Inject]
	public PlayerNamesContainer playerNamesContainer
	{
		private get;
		set;
	}

	[Inject]
	public PlayerTeamsContainer playerTeamsContainer
	{
		get;
		set;
	}

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
	{
		get;
		set;
	}

	[Inject]
	public RigidbodyDataContainer rigidbodyDataContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineSpawnDispatcher machineDispatcher
	{
		private get;
		set;
	}

	[Inject]
	public DestructionReporter destructionReporter
	{
		private get;
		set;
	}

	[Inject]
	public SpotStateObserver spotStateObserver
	{
		private get;
		set;
	}

	[Inject]
	public BattlePlayers battlePlayers
	{
		private get;
		set;
	}

	[Inject]
	public IHudStyleController battleHudStyleController
	{
		private get;
		set;
	}

	public event Action<float> OnMinimapZoom = delegate
	{
	};

	unsafe void IInitialize.OnDependenciesInjected()
	{
		machineDispatcher.OnPlayerRegistered += OnPlayerRegistered;
		machineDispatcher.OnPlayerUnregistered += OnPlayerUnregistered;
		machineDispatcher.OnPlayerRespawnedIn += OnPlayerRespawnedIn;
		destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
		if (WorldSwitching.GetGameModeType() != GameModeType.Campaign)
		{
			spotStateObserver.AddAction(new ObserverAction<SpotStateChangeArgs>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
		_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)UpdateVisibility);
	}

	public void OnFrameworkDestroyed()
	{
		battleHudStyleController.RemoveHud(this);
	}

	public void OnDestroy()
	{
		machineDispatcher.OnPlayerRegistered -= OnPlayerRegistered;
		machineDispatcher.OnPlayerUnregistered -= OnPlayerUnregistered;
		destructionReporter.OnMachineDestroyed -= OnMachineDestroyed;
		machineDispatcher.OnPlayerRespawnedIn -= OnPlayerRespawnedIn;
	}

	public void SetStyle(HudStyle style)
	{
		if (style == HudStyle.HideAllButChat)
		{
			EnableMinimap(enabled: false);
		}
	}

	private void HandleMachineSpotted(int playerId)
	{
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		if (!playerTeamsContainer.IsMe(TargetType.Player, playerId) && _machinesInfo.ContainsKey(activeMachine))
		{
			MinimapPlayerInfo minimapPlayerInfo = _machinesInfo[activeMachine];
			minimapPlayerInfo.isSpotted = true;
			if (!playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId))
			{
				_view.PingSpottedPlayer(playerId);
			}
		}
	}

	private void HandleMachineSpotExpired(int playerId)
	{
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		if (!playerTeamsContainer.IsMe(TargetType.Player, playerId) && _machinesInfo.ContainsKey(activeMachine))
		{
			MinimapPlayerInfo minimapPlayerInfo = _machinesInfo[activeMachine];
			minimapPlayerInfo.isSpotted = false;
		}
	}

	public void RegisterView(MinimapView view)
	{
		_view = view;
		battleHudStyleController.AddHud(this);
	}

	public void RegisterPlayerCamera(SimulationCamera camera)
	{
		_camera = camera;
	}

	public void RegisterBasePosition(Vector3 basePosition, bool isFriendly)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_view.SetBasePosition(WorldToNormalized(basePosition), isFriendly);
	}

	public void RegisterCapturePointPosition(Vector3 basePosition, int index)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_view.SetCapturePointPosition(WorldToNormalized(basePosition), index);
	}

	public void SetCapturePointOwner(bool isMyTeam, int index)
	{
		_view.SetCapturePointTeam(isMyTeam, index);
	}

	public void RegisterEqualizerPosition(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_view.SetEqualizerPosition(WorldToNormalized(position));
	}

	public void SetEqualizerOwner(bool isMyTeam)
	{
		_view.SetEqualizerOwner(isMyTeam);
	}

	public void HideEqualizer()
	{
		_view.HideEqualizer();
	}

	public void SetWorldBounds(Vector3 bottomRight, Vector3 topLeft)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		_bottomRight = bottomRight;
		_topLeft = topLeft;
	}

	public void SetMinimapTexture(Texture minimapTexture)
	{
		_minimapTexture = minimapTexture;
	}

	private void OnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isMe)
		{
			OnPlayerMachineBuilt(spawnInParameters.playerId);
		}
		else
		{
			AddPlayer(spawnInParameters.playerId, spawnInParameters.preloadedMachine.machineId);
		}
	}

	private void OnPlayerUnregistered(UnregisterParametersPlayer unregisterParameters)
	{
		RemovePlayer(unregisterParameters.playerId, unregisterParameters.machineId);
	}

	protected virtual void OnPlayerRespawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isMe)
		{
			_view.RegisterMainPlayer();
			_task.Start((Action<PausableTaskException>)null, (Action)null);
			EnableMinimap(enabled: true);
		}
		else
		{
			_activePlayers.Add(spawnInParameters.playerId);
		}
	}

	protected virtual void OnMachineDestroyed(int playerId, int machineId, bool isMe)
	{
		if (isMe)
		{
			_view.UnregisterMainPlayer();
			if (_task != null)
			{
				_task.Stop();
			}
			EnableMinimap(enabled: false);
		}
		else if (_machinesInfo.ContainsKey(machineId))
		{
			MinimapPlayerInfo playerInfo = _machinesInfo[machineId];
			SetVisible(playerInfo, visible: false);
			_activePlayers.Remove(playerId);
		}
	}

	private void OnPlayerMachineBuilt(int playerId)
	{
		_playerMachineInfo = new MinimapPlayerInfo();
		_playerMachineInfo.playerId = playerId;
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		_playerMachineInfo.rigidbody = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
		_view.RegisterMainPlayer();
		_task.Start((Action<PausableTaskException>)null, (Action)null);
		_view.OnInitialGuiDisabled();
	}

	private void UpdatePlayerMachine()
	{
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, _playerMachineInfo.playerId);
		_playerMachineInfo.rigidbody = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
	}

	private void EnableMinimap(bool enabled)
	{
		_view.get_gameObject().SetActive(enabled);
	}

	public void Start()
	{
		_view.SetTexture(_minimapTexture);
	}

	public void MinimapResized(float pixelOffset)
	{
		this.OnMinimapZoom(pixelOffset);
	}

	private void RemovePlayer(int playerId, int machineId)
	{
		MinimapPlayerInfo playerInfo = _machinesInfo[machineId];
		SetVisible(playerInfo, visible: false);
		_activePlayers.Remove(playerId);
		_view.UnregisterPlayer(playerId);
		_machinesInfo.Remove(machineId);
	}

	private void AddPlayer(int playerId, int machineId)
	{
		_activePlayers.Add(playerId);
		MinimapPlayerInfo minimapPlayerInfo = new MinimapPlayerInfo();
		minimapPlayerInfo.playerId = playerId;
		minimapPlayerInfo.rigidbody = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, machineId);
		_machinesInfo[machineId] = minimapPlayerInfo;
		string playerName = string.Empty;
		playerNamesContainer.TryGetPlayerName(playerId, out playerName);
		int myPlatoonId = battlePlayers.MyPlatoonId;
		int platoonId = battlePlayers.GetPlatoonId(playerName);
		bool isInPlatoon = battlePlayers.GetIsInPlatoon(playerName);
		_view.RegisterPlayer(playerId, playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId), isInPlatoon && myPlatoonId == platoonId);
		if (WorldSwitching.GetGameModeType() == GameModeType.Campaign)
		{
			HandleMachineSpotted(playerId);
		}
	}

	private IEnumerator UpdateVisibility()
	{
		int j = 0;
		while (true)
		{
			j %= 10;
			UpdateMainPlayer();
			int num = 0;
			while (j == 0 && num < _activePlayers.Count)
			{
				int playerId = _activePlayers[num];
				UpdateSingleVisibility(playerId);
				UpdateSingleActivePlayer(playerId);
				num++;
			}
			j++;
			yield return null;
		}
	}

	private void UpdateSingleVisibility(int playerId)
	{
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		if (_machinesInfo.ContainsKey(activeMachine))
		{
			MinimapPlayerInfo minimapPlayerInfo = _machinesInfo[activeMachine];
			if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId))
			{
				SetVisible(minimapPlayerInfo, visible: true);
			}
			else if (!minimapPlayerInfo.isSpotted)
			{
				SetVisible(minimapPlayerInfo, visible: false);
			}
			else
			{
				SetVisible(minimapPlayerInfo, minimapPlayerInfo.isSpotted);
			}
		}
	}

	private void UpdateMainPlayer()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (_playerMachineInfo.rigidbody != null)
		{
			if (!_playerMachineInfo.rigidbody.get_gameObject().get_activeInHierarchy())
			{
				UpdatePlayerMachine();
			}
			Rigidbody rigidbody = _playerMachineInfo.rigidbody;
			Vector2 position = WorldToNormalized(rigidbody.get_worldCenterOfMass());
			Quaternion rotation = _camera.get_transform().get_rotation();
			Vector3 eulerAngles = rotation.get_eulerAngles();
			eulerAngles.z = 0f - eulerAngles.y;
			eulerAngles.x = 0f;
			eulerAngles.y = 0f;
			Quaternion cameraOrientation = Quaternion.Euler(eulerAngles);
			Quaternion rotation2 = rigidbody.get_rotation();
			eulerAngles = rotation2.get_eulerAngles();
			eulerAngles.z = 0f - eulerAngles.y;
			eulerAngles.x = 0f;
			eulerAngles.y = 0f;
			Quaternion orientation = Quaternion.Euler(eulerAngles);
			_view.UpdatePlayerSprite(position, orientation, cameraOrientation);
		}
	}

	private void UpdateSingleActivePlayer(int playerId)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		if (!_machinesInfo.ContainsKey(activeMachine))
		{
			return;
		}
		MinimapPlayerInfo minimapPlayerInfo = _machinesInfo[activeMachine];
		if (minimapPlayerInfo.isVisible)
		{
			Rigidbody rigidbody = minimapPlayerInfo.rigidbody;
			if (rigidbody != null)
			{
				Vector2 position = WorldToNormalized(rigidbody.get_worldCenterOfMass());
				_view.UpdateSprite(playerId, position, Quaternion.get_identity());
			}
		}
	}

	private Vector2 WorldToNormalized(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.get_zero();
		zero.x = (position.x - _bottomRight.x) / (_topLeft.x - _bottomRight.x);
		zero.y = (position.z - _bottomRight.z) / (_topLeft.z - _bottomRight.z);
		return zero;
	}

	protected void SetVisible(MinimapPlayerInfo playerInfo, bool visible)
	{
		if (playerInfo.isVisible != visible)
		{
			playerInfo.isVisible = visible;
			_view.Enable(playerInfo.playerId, playerInfo.isVisible);
			if (visible && playerInfo.isSpotted)
			{
				_view.PingSpottedPlayer(playerInfo.playerId);
			}
		}
	}

	public bool GetVisible(int playerId)
	{
		int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
		return _machinesInfo.ContainsKey(activeMachine) && _machinesInfo[activeMachine].isVisible;
	}
}
