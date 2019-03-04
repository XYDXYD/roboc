using Simulation;
using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Simulation.SinglePlayer;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal sealed class RegisterTutorialAIMachineCommand : ICommand
{
	private string _name;

	private string _displayName;

	private int _player;

	private PreloadedMachine _preloadedMachine;

	private int _team;

	private bool _humanPlayerTeam;

	private SpawningPoint _spawningPoint;

	private string _deathEffect;

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	[Inject]
	public PlayerNamesContainer playerNamesContainer
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
	public PlayerTeamsContainer playerTeamsContainer
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
		get;
		private set;
	}

	[Inject]
	internal ICommandFactory commandFactory
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

	public RegisterTutorialAIMachineCommand Initialise(int player, int team, string name, string displayName, PreloadedMachine preloadedMachine, SpawningPoint spawningPoint, string deathEffect)
	{
		_player = player;
		_team = team;
		_name = name;
		_displayName = displayName;
		_preloadedMachine = preloadedMachine;
		_humanPlayerTeam = false;
		_spawningPoint = spawningPoint;
		_deathEffect = deathEffect;
		return this;
	}

	private void Register()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		RegisterMachine();
		connectedPlayers.PlayerConnected(_player, _name);
		playerNamesContainer.RegisterPlayerName(_player, _name, _displayName);
		playerTeamsContainer.RegisterPlayerTeam(TargetType.Player, _player, _team);
		livePlayersContainer.MarkAsLive(TargetType.Player, _player);
		_preloadedMachine.machineBoard.SetActive(true);
		machineTeamColourUtility.SetRobotTeamColors(isEnemy: true, _preloadedMachine.machineBoard);
		Quaternion spawnRotation = _spawningPoint.get_transform().get_rotation();
		Vector3 spawnPosition = _spawningPoint.get_transform().get_position();
		Rigidbody rbData = _preloadedMachine.rbData;
		MachineSpawnUtility.AdjustSpawnPosition(_preloadedMachine.machineInfo, rbData, ref spawnPosition, ref spawnRotation);
		rbData.set_position(spawnPosition);
		rbData.set_rotation(spawnRotation);
		rbData.set_isKinematic(false);
		SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(_player, _preloadedMachine.machineId, _name, _team, _isMe: false, _humanPlayerTeam, _preloadedMachine, _isAImachine: true, _isLocal: true);
		machineDispatcher.PlayerRegistered(spawnInParameters);
		PlayerIDsDependency dependency = new PlayerIDsDependency(new int[1]
		{
			_player
		});
		commandFactory.Build<SetHostedAIsClientCommand>().Inject(dependency).Execute();
	}

	private void RegisterMachine()
	{
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		GameObject machineBoard = _preloadedMachine.machineBoard;
		int machineId = _preloadedMachine.machineId;
		playerMachinesContainer.RegisterPlayerMachine(TargetType.Player, _player, machineId);
		AIWeaponRaycast component = _preloadedMachine.rbData.GetComponent<AIWeaponRaycast>();
		weaponRaycastContainer.RegisterWeaponRaycast(machineId, component);
		machineRootContainer.RegisterMachineRoot(TargetType.Player, machineId, machineBoard);
		rigidbodyDataContainer.RegisterRigidBodyData(TargetType.Player, machineId, _preloadedMachine.rbData);
		machineClusterContainer.RegisterMachineCluster(TargetType.Player, machineId, _preloadedMachine.machineGraph.cluster);
		machineClusterContainer.RegisterMicrobotCollisionSphere(TargetType.Player, machineId, _preloadedMachine.machineGraph.sphere);
		machineManager.RegisterMachineMap(TargetType.Player, machineId, _preloadedMachine.machineMap);
		AIInputWrapper aIInputWrapper = _preloadedMachine.inputWrapper as AIInputWrapper;
		MachineOwnerImplementor machineOwnerImplementor = new MachineOwnerImplementor();
		machineOwnerImplementor.SetOwnedByMe(ownedByMe_: false);
		machineOwnerImplementor.SetOwnedByAi(ownedByAi_: true);
		machineOwnerImplementor.SetOwner(_player, machineId);
		machineOwnerImplementor.ownerTeamId = _team;
		MachineInputImplementor machineInputImplementor = new MachineInputImplementor(_preloadedMachine.inputWrapper);
		MachineStunImplementor machineStunImplementor = new MachineStunImplementor();
		MachineRigidbodyTransformImplementor machineRigidbodyTransformImplementor = new MachineRigidbodyTransformImplementor(_preloadedMachine.rbData);
		MachineTopSpeedImplementor machineTopSpeedImplementor = new MachineTopSpeedImplementor();
		AutoHealImplementor autoHealImplementor = new AutoHealImplementor(machineId);
		GameObject val = gameobjectFactory.Build(_deathEffect);
		DeathEffectImplementor component2 = val.GetComponent<DeathEffectImplementor>();
		val.SetActive(false);
		DeathEffectDependenciesImplementor deathEffectDependenciesImplementor = new DeathEffectDependenciesImplementor(_preloadedMachine.rbData, _preloadedMachine.rbData.get_transform().get_parent().get_gameObject(), _preloadedMachine.machineInfo.MachineCenter, _preloadedMachine.machineInfo.MachineSize);
		FasterList<object> val2 = new FasterList<object>();
		val2.Add((object)machineStunImplementor);
		val2.Add((object)machineRigidbodyTransformImplementor);
		val2.Add((object)machineOwnerImplementor);
		val2.Add((object)machineInputImplementor);
		val2.Add((object)autoHealImplementor);
		val2.Add((object)aIInputWrapper);
		val2.Add((object)machineTopSpeedImplementor);
		val2.Add((object)new MachineFunctionalImplementor(machineId));
		val2.Add((object)new WeaponOrderImplementor(_preloadedMachine.weaponOrder));
		val2.Add((object)new InputMotorComponent());
		val2.Add((object)deathEffectDependenciesImplementor);
		val2.Add((object)component2);
		FasterList<object> val3 = val2;
		FasterList<IEntityViewBuilder> val4 = new FasterList<IEntityViewBuilder>();
		RegistrationHelper.CheckCubesNodes(_preloadedMachine, val3, val4, isLocalPlayer: false);
		engineRoot.BuildEntity(machineId, new DynamicEntityDescriptorInfo<TutorialDummyMachineEntityDescriptor>(val4), val3.ToArray());
	}
}
