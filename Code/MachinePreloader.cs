using Authentication;
using Battle;
using Simulation;
using Simulation.SinglePlayer;
using Svelto.Command;
using Svelto.IoC;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

internal class MachinePreloader
{
	private readonly Dictionary<string, PreloadedMachine> _machinesPerPlayerName = new Dictionary<string, PreloadedMachine>();

	private readonly List<PreloadedMachine> _allPreloadedMachines = new List<PreloadedMachine>();

	public bool IsComplete;

	private float _progress;

	[Inject]
	public MachineSimulationBuilder machineSimulationBuilder
	{
		protected get;
		set;
	}

	[Inject]
	public BattlePlayers battlePlayers
	{
		protected get;
		set;
	}

	[Inject]
	public PlayerNamesContainer playerNamesContainer
	{
		private get;
		set;
	}

	[Inject]
	public ICommandFactory commandFactory
	{
		private get;
		set;
	}

	[Inject]
	public PlayerMachineBuiltListener playerMachineListener
	{
		protected get;
		set;
	}

	[Inject]
	public LocalAIsContainer localAIs
	{
		private get;
		set;
	}

	[Inject]
	public AIPreloadRobotBuilder aiPreloadRobotBuilder
	{
		private get;
		set;
	}

	public float Progress
	{
		get
		{
			if (IsComplete)
			{
				return 1f;
			}
			return _progress;
		}
	}

	public event Action OnEachMachinePreloaded = delegate
	{
	};

	public PreloadedMachine GetPreloadedMachine(string playerName)
	{
		return _machinesPerPlayerName[playerName];
	}

	public PreloadedMachine GetPreloadedMachine(int machineId)
	{
		for (int i = 0; i < _allPreloadedMachines.Count; i++)
		{
			if (_allPreloadedMachines[i].machineId == machineId)
			{
				return _allPreloadedMachines[i];
			}
		}
		Console.LogError("MachinePreloader could not find machine " + machineId);
		return null;
	}

	public List<PreloadedMachine> GetAllPreloadedMachines()
	{
		return _allPreloadedMachines;
	}

	public IEnumerator PreloadAllMachines()
	{
		yield return PreloadAllMachines(excludePlayerMachine: false);
	}

	public IEnumerator PreloadAllEnemyMachines()
	{
		yield return PreloadAllMachines(excludePlayerMachine: true);
	}

	private IEnumerator PreloadAllMachines(bool excludePlayerMachine)
	{
		foreach (KeyValuePair<string, PreloadedMachine> item in _machinesPerPlayerName)
		{
			if (!(item.Key != User.Username))
			{
				KeyValuePair<string, PreloadedMachine> keyValuePair = new KeyValuePair<string, PreloadedMachine>(item.Key, item.Value);
				_machinesPerPlayerName.Clear();
				_allPreloadedMachines.Clear();
				_machinesPerPlayerName.Add(keyValuePair.Key, keyValuePair.Value);
				_allPreloadedMachines.Add(keyValuePair.Value);
				break;
			}
		}
		List<PlayerDataDependency> list = battlePlayers.GetExpectedPlayersList();
		int count = list.Count;
		for (int i = 0; i < list.Count; i++)
		{
			if (!excludePlayerMachine || list[i].PlayerName != User.Username)
			{
				yield return Build(list[i]);
				yield return null;
				this.OnEachMachinePreloaded();
				_progress = (float)(i + 1) / (float)count;
			}
		}
		IsComplete = true;
	}

	private IEnumerator Build(PlayerDataDependency playerData)
	{
		string name = playerData.PlayerName;
		Console.Log("Preloader registered machine from '" + name + "'");
		Byte3 gridOffset = new Byte3(8, 8, 8);
		PreloadedMachine preloadedMachine = new PreloadedMachine();
		preloadedMachine.machineModel = new MachineModel(playerData.RobotData, gridOffset);
		preloadedMachine.machineModel.SetColorData(playerData.RobotColourData);
		preloadedMachine.weaponOrder = playerData.WeaponOrder;
		RBEntity rbentity = PhysicsActivator.ActivatePhysicsKinematic();
		preloadedMachine.rbData = rbentity.rigidBody;
		preloadedMachine.machineBoard = rbentity.board;
		preloadedMachine.machineBoard.SetActive(false);
		int playerId = playerNamesContainer.GetPlayerId(playerData.PlayerName);
		yield return BuildMachine(name, preloadedMachine, playerId, playerData.AiPlayer);
		preloadedMachine.machineId = PlayerMachinesContainer.GetPrimaryMachineId(playerId);
		_machinesPerPlayerName.Add(name, preloadedMachine);
		_allPreloadedMachines.Add(preloadedMachine);
		BuildMachineCommandDependency dep = new BuildMachineCommandDependency(playerId, playerData.PlayerName, playerData.TeamId, playerData.AiPlayer, playerData.SpawnEffect, playerData.DeathEffect);
		if (playerData.PlayerName == User.Username)
		{
			BuildOwnMachineCommand buildOwnMachineCommand = commandFactory.Build<BuildOwnMachineCommand>();
			buildOwnMachineCommand.Inject(dep);
			buildOwnMachineCommand.Execute();
		}
		else if (!localAIs.IsAIHostedLocally(playerId))
		{
			BuildExistingMachineClientCommand buildExistingMachineClientCommand = commandFactory.Build<BuildExistingMachineClientCommand>();
			buildExistingMachineClientCommand.Inject(dep);
			buildExistingMachineClientCommand.Execute();
		}
	}

	private IEnumerator BuildMachine(string name, PreloadedMachine preloadedMachine, int playerId, bool isAI)
	{
		yield return machineSimulationBuilder.SetupSimulationMachine(preloadedMachine);
		if (name == User.Username)
		{
			playerMachineListener.PlayerRobotBuilt(preloadedMachine);
			yield break;
		}
		if (localAIs.IsAIHostedLocally(playerId))
		{
			aiPreloadRobotBuilder.GenerateWeaponOrder(preloadedMachine);
			playerMachineListener.AiRobotBuilt(preloadedMachine);
			yield break;
		}
		if (preloadedMachine.weaponOrder == null)
		{
			if (!isAI)
			{
				throw new Exception("Remote machine doesn't have weapon order");
			}
			aiPreloadRobotBuilder.GenerateWeaponOrder(preloadedMachine);
		}
		playerMachineListener.RemoteRobotBuilt(preloadedMachine);
	}
}
