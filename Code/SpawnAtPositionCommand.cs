using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

internal sealed class SpawnAtPositionCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private SpawnPointDependency _dependency;

	[Inject]
	private MachinePreloader machinePreloader
	{
		get;
		set;
	}

	[Inject]
	private PlayerTeamsContainer teamsContainer
	{
		get;
		set;
	}

	[Inject]
	private LivePlayersContainer livePlayers
	{
		get;
		set;
	}

	[Inject]
	private MachineSpawnDispatcher networkMachineDispatcher
	{
		get;
		set;
	}

	[Inject]
	internal PlayerNamesContainer playerNamesContainer
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as SpawnPointDependency);
		return this;
	}

	public void Execute()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		int owner = _dependency.owner;
		int primaryMachineId = PlayerMachinesContainer.GetPrimaryMachineId(_dependency.owner);
		PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(primaryMachineId);
		preloadedMachine.machineBoard.SetActive(true);
		Rigidbody rbData = preloadedMachine.rbData;
		MachineSpawnUtility.AdjustSpawnPosition(preloadedMachine.machineInfo, rbData, ref _dependency.position, ref _dependency.rotation);
		rbData.set_position(_dependency.position);
		rbData.set_rotation(_dependency.rotation);
		livePlayers.MarkAsLive(TargetType.Player, owner);
		string playerName = playerNamesContainer.GetPlayerName(owner);
		SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(owner, primaryMachineId, playerNamesContainer.GetPlayerName(owner), teamsContainer.GetPlayerTeam(TargetType.Player, owner), teamsContainer.IsMe(TargetType.Player, owner), teamsContainer.IsOnMyTeam(TargetType.Player, owner), preloadedMachine, _isAImachine: false, teamsContainer.IsMe(TargetType.Player, owner));
		networkMachineDispatcher.PlayerSpawnedIn(spawnInParameters);
	}
}
