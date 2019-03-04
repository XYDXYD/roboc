using Simulation;
using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.PowerConsumption;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.Shooting;
using Simulation.SpawnEffects;
using SinglePlayerCampaign.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.Implementors;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal sealed class RegisterAIMachineCommand : ICommand
{
	private string _name;

	private int _player;

	private PreloadedMachine _preloadedMachine;

	private int _team;

	private float _maxSpeed;

	private float _maxTurningSpeed;

	private bool _humanPlayerTeam;

	private int _campaignSpawnEventId;

	private bool _isKillRequirement;

	private string _spawnEffect;

	private string _deathEffect;

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineClusterContainer machineClusterContainer
	{
		private get;
		set;
	}

	[Inject]
	public RigidbodyDataContainer rigidbodyDataContainer
	{
		private get;
		set;
	}

	[Inject]
	public WeaponRaycastContainer weaponRaycastContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineRootContainer machineRootContainer
	{
		private get;
		set;
	}

	[Inject]
	public NetworkMachineManager machineManager
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
	public RegisterPlayerObserver registerPlayerObserver
	{
		private get;
		set;
	}

	[Inject]
	public ConnectedPlayersContainer connectedPlayers
	{
		private get;
		set;
	}

	[Inject]
	public MachineTeamColourUtility machineTeamColourUtility
	{
		private get;
		set;
	}

	[Inject]
	public IEntityFactory engineRoot
	{
		private get;
		set;
	}

	[Inject]
	public IGameObjectFactory gameobjectFactory
	{
		private get;
		set;
	}

	public void Execute()
	{
		Register();
	}

	public RegisterAIMachineCommand Initialise(int player, int team, string name, PreloadedMachine preloadedMachine, float maxSpeed, float maxTurningSpeed, bool humanPlayerTeam, string spawnEffect, string deathEffect, int campaignSpawnEventId = -1, bool isKillRequirement = true)
	{
		_player = player;
		_team = team;
		_name = name;
		_preloadedMachine = preloadedMachine;
		_maxSpeed = maxSpeed;
		_maxTurningSpeed = maxTurningSpeed;
		_humanPlayerTeam = humanPlayerTeam;
		_campaignSpawnEventId = campaignSpawnEventId;
		_isKillRequirement = isKillRequirement;
		_spawnEffect = spawnEffect;
		_deathEffect = deathEffect;
		return this;
	}

	private void Register()
	{
		_preloadedMachine.machineBoard.SetActive(false);
		RegisterMachine();
		connectedPlayers.PlayerConnected(_player, _name);
		machineTeamColourUtility.SetRobotTeamColors(!_humanPlayerTeam, _preloadedMachine.machineBoard);
		SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(_player, _preloadedMachine.machineId, _name, _team, _isMe: false, _humanPlayerTeam, _preloadedMachine, _isAImachine: true, _isLocal: true);
		machineDispatcher.PlayerRegistered(spawnInParameters);
		registerPlayerObserver.RegisterPlayer(_name, _player, isMe: false, _humanPlayerTeam);
	}

	private void RegisterMachine()
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		GameObject machineBoard = _preloadedMachine.machineBoard;
		int machineId = _preloadedMachine.machineId;
		Rigidbody rbData = _preloadedMachine.rbData;
		playerMachinesContainer.RegisterPlayerMachine(TargetType.Player, _player, machineId);
		AIWeaponRaycast weaponRaycast = _preloadedMachine.weaponRaycast as AIWeaponRaycast;
		weaponRaycastContainer.RegisterWeaponRaycast(machineId, weaponRaycast);
		machineRootContainer.RegisterMachineRoot(TargetType.Player, machineId, machineBoard);
		rigidbodyDataContainer.RegisterRigidBodyData(TargetType.Player, machineId, rbData);
		machineClusterContainer.RegisterMachineCluster(TargetType.Player, machineId, _preloadedMachine.machineGraph.cluster);
		machineClusterContainer.RegisterMicrobotCollisionSphere(TargetType.Player, machineId, _preloadedMachine.machineGraph.sphere);
		machineManager.RegisterMachineMap(TargetType.Player, machineId, _preloadedMachine.machineMap);
		AIEnemyBehaviorTreeImplementor aIEnemyBehaviorTreeImplementor = machineBoard.AddComponent<AIEnemyBehaviorTreeImplementor>();
		aIEnemyBehaviorTreeImplementor.LoadData(WorldSwitching.GetGameModeType());
		AIGameObjectMovementData aIGameObjectMovementData = new AIGameObjectMovementData();
		aIGameObjectMovementData.playeId = _player;
		aIGameObjectMovementData.playerName = _name;
		aIGameObjectMovementData.teamId = _team;
		AIInputWrapper aIInputWrapper = _preloadedMachine.inputWrapper as AIInputWrapper;
		Vector3 val = _preloadedMachine.machineInfo.MachineSize * 0.5f;
		Vector3 machineCenter = _preloadedMachine.machineInfo.MachineCenter;
		Vector3[] minmax_ = (Vector3[])new Vector3[2]
		{
			machineCenter - val,
			machineCenter + val
		};
		val.y = 0f;
		float magnitude = val.get_magnitude();
		aIGameObjectMovementData.SetRigidBody(rbData);
		aIGameObjectMovementData.SetHorizontalRadius(magnitude);
		aIGameObjectMovementData.SetMaxSpeed(_maxSpeed);
		aIGameObjectMovementData.SetMaxTurningSpeed(_maxTurningSpeed);
		aIGameObjectMovementData.SetminMax(minmax_);
		aIGameObjectMovementData.SetRoot(machineBoard);
		AIWeaponShootingFeedbackComponent aIWeaponShootingFeedbackComponent = new AIWeaponShootingFeedbackComponent();
		aIEnemyBehaviorTreeImplementor.SetBehaviorGlobalVariables(aIGameObjectMovementData, aIInputWrapper, weaponRaycast, _preloadedMachine.machineInfo.cameraPivotTransform);
		MachineOwnerImplementor machineOwnerImplementor = new MachineOwnerImplementor();
		machineOwnerImplementor.SetOwnedByMe(ownedByMe_: false);
		machineOwnerImplementor.SetOwnedByAi(ownedByAi_: true);
		machineOwnerImplementor.SetOwner(_player, machineId);
		machineOwnerImplementor.ownerTeamId = _team;
		MachineInputImplementor machineInputImplementor = new MachineInputImplementor(_preloadedMachine.inputWrapper);
		MachineRaycastImplementor machineRaycastImplementor = new MachineRaycastImplementor(_preloadedMachine.weaponRaycast);
		WeaponOrderImplementor weaponOrderImplementor = new WeaponOrderImplementor(_preloadedMachine.weaponOrder);
		MachineStunImplementor machineStunImplementor = new MachineStunImplementor();
		MachineRigidbodyTransformImplementor machineRigidbodyTransformImplementor = new MachineRigidbodyTransformImplementor(rbData);
		AssistBonusComponent assistBonusComponent = new AssistBonusComponent();
		AutoHealImplementor autoHealImplementor = new AutoHealImplementor(machineId);
		ProtectTeamMateBonusComponent protectTeamMateBonusComponent = new ProtectTeamMateBonusComponent();
		HealAssistComponent healAssistComponent = new HealAssistComponent();
		AIScoreComponent aIScoreComponent = new AIScoreComponent();
		AIBotIdData aIBotIdData = new AIBotIdData(_player, _team, machineId);
		aIGameObjectMovementData.cameraPivotTransform = _preloadedMachine.machineInfo.cameraPivotTransform;
		AIPowerConsumptionComponent aIPowerConsumptionComponent = new AIPowerConsumptionComponent();
		AIEquippedWeaponComponent aIEquippedWeaponComponent = new AIEquippedWeaponComponent();
		AudioGameObjectComponentImplementor audioGameObjectComponentImplementor = new AudioGameObjectComponentImplementor(_preloadedMachine.machineInfo.centerTransform.get_gameObject());
		SpottableImplementor spottableImplementor = new SpottableImplementor(machineId);
		AIPlayerSpotterImplementor aIPlayerSpotterImplementor = machineBoard.AddComponent<AIPlayerSpotterImplementor>();
		AliveStateImplementor aliveStateImplementor = new AliveStateImplementor(machineId);
		aliveStateImplementor.isAlive.set_value(true);
		MachineInvisibilityImplementor machineInvisibilityImplementor = new MachineInvisibilityImplementor();
		MachineTopSpeedImplementor machineTopSpeedImplementor = new MachineTopSpeedImplementor();
		MachineHealingPriorityImplementor machineHealingPriorityImplementor = new MachineHealingPriorityImplementor();
		MachineFunctionalImplementor machineFunctionalImplementor = new MachineFunctionalImplementor(machineId);
		MachineMapImplementor machineMapImplementor = new MachineMapImplementor();
		machineMapImplementor.machineMap = _preloadedMachine.machineMap;
		MachineMapImplementor machineMapImplementor2 = machineMapImplementor;
		DamagedByImplementor damagedByImplementor = new DamagedByImplementor();
		MachineWeaponsBlockedImplementor machineWeaponsBlockedImplementor = new MachineWeaponsBlockedImplementor();
		MachineTargetsImplementor machineTargetsImplementor = new MachineTargetsImplementor();
		GameObject val2 = gameobjectFactory.Build(_spawnEffect);
		SpawnEffectImplementor component = val2.GetComponent<SpawnEffectImplementor>();
		val2.SetActive(false);
		SpawnEffectDependenciesImplementor spawnEffectDependenciesImplementor = new SpawnEffectDependenciesImplementor(rbData, _preloadedMachine.machineInfo.MachineCenter, _preloadedMachine.machineInfo.MachineSize, _preloadedMachine.allRenderers);
		val2 = gameobjectFactory.Build(_deathEffect);
		DeathEffectImplementor component2 = val2.GetComponent<DeathEffectImplementor>();
		val2.SetActive(false);
		DeathEffectDependenciesImplementor deathEffectDependenciesImplementor = new DeathEffectDependenciesImplementor(rbData, rbData.get_transform().get_parent().get_gameObject(), _preloadedMachine.machineInfo.MachineCenter, _preloadedMachine.machineInfo.MachineSize);
		FasterList<object> val3 = new FasterList<object>();
		val3.Add((object)aIEnemyBehaviorTreeImplementor);
		val3.Add((object)machineStunImplementor);
		val3.Add((object)machineRigidbodyTransformImplementor);
		val3.Add((object)machineOwnerImplementor);
		val3.Add((object)machineInputImplementor);
		val3.Add((object)aIBotIdData);
		val3.Add((object)aIGameObjectMovementData);
		val3.Add((object)assistBonusComponent);
		val3.Add((object)autoHealImplementor);
		val3.Add((object)protectTeamMateBonusComponent);
		val3.Add((object)healAssistComponent);
		val3.Add((object)aIInputWrapper);
		val3.Add((object)aIWeaponShootingFeedbackComponent);
		val3.Add((object)aIPowerConsumptionComponent);
		val3.Add((object)aIEquippedWeaponComponent);
		val3.Add((object)aIScoreComponent);
		val3.Add((object)audioGameObjectComponentImplementor);
		val3.Add((object)spottableImplementor);
		val3.Add((object)aIPlayerSpotterImplementor);
		val3.Add((object)aliveStateImplementor);
		val3.Add((object)machineInvisibilityImplementor);
		val3.Add((object)machineHealingPriorityImplementor);
		val3.Add((object)machineFunctionalImplementor);
		val3.Add((object)machineRaycastImplementor);
		val3.Add((object)weaponOrderImplementor);
		val3.Add((object)machineTopSpeedImplementor);
		val3.Add((object)machineMapImplementor2);
		val3.Add((object)damagedByImplementor);
		val3.Add((object)machineWeaponsBlockedImplementor);
		val3.Add((object)machineTargetsImplementor);
		val3.Add((object)new InputMotorComponent());
		val3.Add((object)component);
		val3.Add((object)spawnEffectDependenciesImplementor);
		val3.Add((object)component2);
		val3.Add((object)deathEffectDependenciesImplementor);
		val3.Add((object)new EntitySourceComponent(isLocal: true));
		val3.Add((object)new SpawnableImplementor(machineId));
		FasterList<object> val4 = val3;
		FasterList<IEntityViewBuilder> val5 = new FasterList<IEntityViewBuilder>();
		RegistrationHelper.CheckCubesNodes(_preloadedMachine, val4, val5, isLocalPlayer: false);
		RegistrationHelper.CheckForSpecificGameModeViews(WorldSwitching.GetGameModeType(), val4, val5);
		if (_campaignSpawnEventId >= 0)
		{
			val4.Add((object)new SpawnEventIdImplementor(_campaignSpawnEventId));
			val5.Add(new EntityViewBuilder<AIMachineRespawnEntityView>());
			val4.Add((object)new IsKillRequirementImplementor(_isKillRequirement));
			val5.Add(new EntityViewBuilder<AIMachineKillRequirementEntityView>());
			val5.Add(new EntityViewBuilder<AIMachineDespawnEntityView>());
		}
		if (val5.get_Count() != 0)
		{
			DynamicEntityDescriptorInfo<AIMachineEntityDescriptor> val6 = new DynamicEntityDescriptorInfo<AIMachineEntityDescriptor>(val5);
			engineRoot.BuildEntity(machineId, val6, val4.ToArray());
		}
		else
		{
			engineRoot.BuildEntity<AIMachineEntityDescriptor>(machineId, val4.ToArray());
		}
	}
}
