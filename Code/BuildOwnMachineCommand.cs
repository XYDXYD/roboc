using Authentication;
using Battle;
using Simulation;
using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Simulation.SinglePlayer;
using Simulation.SpawnEffects;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;
using Utility;

internal sealed class BuildOwnMachineCommand : IInjectableCommand<BuildMachineCommandDependency>, ICommand
{
	private BuildMachineCommandDependency _dependency;

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
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

	[Inject]
	public NetworkMachineManager machineManager
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
	public MachineRootContainer machineRootContainer
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
	public MachineSpawnDispatcher networkMachineDispatcher
	{
		private get;
		set;
	}

	[Inject]
	public MachinePreloader machinePreloader
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
	public RegisterPlayerObserver registerPlayerObserver
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
	public BattlePlayers battlePlayers
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

	public ICommand Inject(BuildMachineCommandDependency dependency)
	{
		_dependency = dependency;
		return this;
	}

	public void Execute()
	{
		Execute(_dependency.playerId, _dependency.spawnEffect, _dependency.deathEffect);
	}

	private void Execute(int localId, string spawnEffect, string deathEffect)
	{
		Console.Log("Build own machine");
		int myTeam = (int)battlePlayers.MyTeam;
		string username = User.Username;
		connectedPlayers.PlayerConnected(localId, username);
		PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(username);
		preloadedMachine.machineBoard.SetActive(false);
		int masteryLevel = battlePlayers.GetMasteryLevel(username);
		GameObject gameObject = preloadedMachine.rbData.get_gameObject();
		BuildMachineEntity(localId, preloadedMachine, gameObject, myTeam, masteryLevel, spawnEffect, deathEffect);
		machineTeamColourUtility.SetRobotTeamColors(isEnemy: false, preloadedMachine.machineBoard);
		SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(localId, preloadedMachine.machineId, User.Username, myTeam, _isMe: true, _isOnMyTeam: true, preloadedMachine, _isAImachine: false, _isLocal: true);
		networkMachineDispatcher.PlayerRegistered(spawnInParameters);
		registerPlayerObserver.RegisterPlayer(username, localId, isMe: true, isMyTeam: true);
		SetupDebugInfo(localId, preloadedMachine);
	}

	private void SetupDebugInfo(int localId, PreloadedMachine preloadedMachine)
	{
	}

	private void BuildMachineEntity(int playerId, PreloadedMachine preloadedMachine, GameObject machineRoot, int teamId, int masteryLevel, string spawnEffect, string deathEffect)
	{
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		GameObject machineBoard = preloadedMachine.machineBoard;
		int machineId = preloadedMachine.machineId;
		Rigidbody rbData = preloadedMachine.rbData;
		playerMachinesContainer.RegisterPlayerMachine(TargetType.Player, playerId, machineId);
		machineClusterContainer.RegisterMachineCluster(TargetType.Player, machineId, preloadedMachine.machineGraph.cluster);
		machineClusterContainer.RegisterMicrobotCollisionSphere(TargetType.Player, machineId, preloadedMachine.machineGraph.sphere);
		machineRootContainer.RegisterMachineRoot(TargetType.Player, machineId, machineBoard);
		machineManager.RegisterMachineMap(TargetType.Player, machineId, preloadedMachine.machineMap);
		rigidbodyDataContainer.RegisterRigidBodyData(TargetType.Player, machineId, rbData);
		MachineOwnerImplementor machineOwnerImplementor = new MachineOwnerImplementor();
		machineOwnerImplementor.SetOwnedByMe(ownedByMe_: true);
		machineOwnerImplementor.SetOwnedByAi(ownedByAi_: false);
		machineOwnerImplementor.SetOwner(playerId, machineId);
		machineOwnerImplementor.ownerTeamId = teamId;
		MachineStunImplementor machineStunImplementor = new MachineStunImplementor();
		MachineRigidbodyTransformImplementor machineRigidbodyTransformImplementor = new MachineRigidbodyTransformImplementor(rbData);
		AudioGameObjectComponentImplementor audioGameObjectComponentImplementor = new AudioGameObjectComponentImplementor(preloadedMachine.machineInfo.centerTransform.get_gameObject());
		SpottableImplementor spottableImplementor = new SpottableImplementor(machineId);
		AliveStateImplementor aliveStateImplementor = new AliveStateImplementor(machineId);
		aliveStateImplementor.isAlive.set_value(true);
		LocalPlayerSpotterImplementor localPlayerSpotterImplementor = new LocalPlayerSpotterImplementor();
		PlayerRobotMasteryComponent playerRobotMasteryComponent = new PlayerRobotMasteryComponent(masteryLevel);
		MachineWeaponsBlockedImplementor machineWeaponsBlockedImplementor = new MachineWeaponsBlockedImplementor();
		MachineHealingPriorityImplementor machineHealingPriorityImplementor = new MachineHealingPriorityImplementor();
		MachineInvisibilityImplementor machineInvisibilityImplementor = new MachineInvisibilityImplementor();
		MachineGroundedImplementor machineGroundedImplementor = new MachineGroundedImplementor();
		MachineTopSpeedImplementor machineTopSpeedImplementor = new MachineTopSpeedImplementor();
		MachineFunctionalImplementor machineFunctionalImplementor = new MachineFunctionalImplementor(machineId);
		MachineInputImplementor machineInputImplementor = new MachineInputImplementor(preloadedMachine.inputWrapper);
		MachineRaycastImplementor machineRaycastImplementor = new MachineRaycastImplementor(preloadedMachine.weaponRaycast);
		WeaponOrderImplementor weaponOrderImplementor = new WeaponOrderImplementor(preloadedMachine.weaponOrder);
		DamagedByImplementor damagedByImplementor = new DamagedByImplementor();
		AutoHealImplementor autoHealImplementor = new AutoHealImplementor(machineId);
		AutoHealGuiImplementor autoHealGuiImplementor = new AutoHealGuiImplementor();
		HealAssistComponent healAssistComponent = new HealAssistComponent();
		MachineTargetsImplementor machineTargetsImplementor = new MachineTargetsImplementor();
		DeltaTimeComponent deltaTimeComponent = new DeltaTimeComponent();
		GameObject val = gameobjectFactory.Build(spawnEffect);
		SpawnEffectImplementor component = val.GetComponent<SpawnEffectImplementor>();
		val.SetActive(false);
		SpawnEffectDependenciesImplementor spawnEffectDependenciesImplementor = new SpawnEffectDependenciesImplementor(rbData, preloadedMachine.machineInfo.MachineCenter, preloadedMachine.machineInfo.MachineSize, preloadedMachine.allRenderers);
		val = gameobjectFactory.Build(deathEffect);
		DeathEffectImplementor component2 = val.GetComponent<DeathEffectImplementor>();
		val.SetActive(false);
		DeathEffectDependenciesImplementor deathEffectDependenciesImplementor = new DeathEffectDependenciesImplementor(rbData, rbData.get_transform().get_parent().get_gameObject(), preloadedMachine.machineInfo.MachineCenter, preloadedMachine.machineInfo.MachineSize);
		PlayerTargetGameObject playerTargetGameObject = new PlayerTargetGameObject(rbData, playerId, teamId, machineId, preloadedMachine.machineInfo.MachineSize.get_magnitude() * 0.5f, preloadedMachine.machineMap, machineInvisibilityImplementor);
		FasterList<object> val2 = new FasterList<object>();
		val2.Add((object)machineInputImplementor);
		val2.Add((object)machineInvisibilityImplementor);
		val2.Add((object)machineGroundedImplementor);
		val2.Add((object)machineOwnerImplementor);
		val2.Add((object)machineStunImplementor);
		val2.Add((object)machineRigidbodyTransformImplementor);
		val2.Add((object)audioGameObjectComponentImplementor);
		val2.Add((object)playerTargetGameObject);
		val2.Add((object)machineWeaponsBlockedImplementor);
		val2.Add((object)spottableImplementor);
		val2.Add((object)aliveStateImplementor);
		val2.Add((object)localPlayerSpotterImplementor);
		val2.Add((object)machineTopSpeedImplementor);
		val2.Add((object)machineHealingPriorityImplementor);
		val2.Add((object)machineFunctionalImplementor);
		val2.Add((object)machineRaycastImplementor);
		val2.Add((object)weaponOrderImplementor);
		val2.Add((object)playerRobotMasteryComponent);
		val2.Add((object)damagedByImplementor);
		val2.Add((object)autoHealImplementor);
		val2.Add((object)autoHealGuiImplementor);
		val2.Add((object)healAssistComponent);
		val2.Add((object)machineTargetsImplementor);
		val2.Add((object)new InputMotorComponent());
		val2.Add((object)component);
		val2.Add((object)spawnEffectDependenciesImplementor);
		val2.Add((object)component2);
		val2.Add((object)deathEffectDependenciesImplementor);
		val2.Add((object)new EntitySourceComponent(isLocal: true));
		val2.Add((object)deltaTimeComponent);
		val2.Add((object)new SpawnableImplementor(machineId));
		FasterList<object> val3 = val2;
		FasterList<IEntityViewBuilder> val4 = new FasterList<IEntityViewBuilder>();
		RegistrationHelper.CheckCubesNodes(preloadedMachine, val3, val4, isLocalPlayer: true);
		RegistrationHelper.CheckForSpecificGameModeViews(WorldSwitching.GetGameModeType(), val3, val4);
		if (val4.get_Count() != 0)
		{
			engineRoot.BuildEntity(machineId, new DynamicEntityDescriptorInfo<LocalMachineEntityDescriptor>(val4), val3.ToArray());
		}
		else
		{
			engineRoot.BuildEntity<LocalMachineEntityDescriptor>(machineId, val3.ToArray());
		}
	}
}
