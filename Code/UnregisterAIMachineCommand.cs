using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;

internal sealed class UnregisterAIMachineCommand : ICommand
{
	private IEntityViewsDB _entityViewsDB;

	private int _player;

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
	public IEntityFunctions entityFunctions
	{
		private get;
		set;
	}

	public void Execute()
	{
		Unregister();
	}

	public UnregisterAIMachineCommand Initialise(int player, IEntityViewsDB entityViewsDB)
	{
		_player = player;
		_entityViewsDB = entityViewsDB;
		return this;
	}

	private void Unregister()
	{
		if (playerMachinesContainer.HasPlayerRegisteredMachine(TargetType.Player, _player))
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, _player);
			UnregisterParametersPlayer unregisterParameters = new UnregisterParametersPlayer(_player, activeMachine);
			weaponRaycastContainer.UnregisterWeaponRaycast(activeMachine);
			machineRootContainer.UnregisterMachineRoot(TargetType.Player, activeMachine);
			rigidbodyDataContainer.UnregisterRigidBodyData(TargetType.Player, activeMachine);
			machineClusterContainer.UnregisterMachineCluster(TargetType.Player, activeMachine);
			machineClusterContainer.UnregisterMicrobotCollisionSphere(TargetType.Player, activeMachine);
			machineManager.UnregisterMachineMap(TargetType.Player, activeMachine);
			playerMachinesContainer.UnregisterPlayerMachine(TargetType.Player, _player);
			machineDispatcher.PlayerUnregistered(unregisterParameters);
			RemoveEntityNode removeEntityNode = _entityViewsDB.QueryEntityView<RemoveEntityNode>(activeMachine);
			entityFunctions.RemoveEntity(activeMachine, removeEntityNode.removeEntityComponent);
			entityFunctions.RemoveGroupedEntities(activeMachine);
		}
	}
}
