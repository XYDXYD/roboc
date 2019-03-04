using LobbyServiceLayer;
using Simulation;
using Simulation.BattleArena;
using Simulation.BattleArena.CapturePoints;
using Simulation.BattleArena.Equalizer;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal sealed class TeamBasePreloader
{
	private const int NUM_TEAMS = 2;

	private Dictionary<int, PreloadedMachine> _teamBases = new Dictionary<int, PreloadedMachine>();

	private PreloadedMachine _equalizer;

	[Inject]
	public MachineSimulationBuilder machineSimulationBuilder
	{
		get;
		set;
	}

	[Inject]
	public WeaponListUtility weaponListUtility
	{
		get;
		set;
	}

	[Inject]
	public IServiceRequestFactory serviceRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	private ILobbyRequestFactory lobbyRequestFactory
	{
		get;
		set;
	}

	[Inject]
	private IEntityFactory entityFactory
	{
		get;
		set;
	}

	[Inject]
	private ICommandFactory commandFactory
	{
		get;
		set;
	}

	[Inject]
	private IGameObjectFactory gameObjectFactory
	{
		get;
		set;
	}

	[Inject]
	private PlayerMachinesContainer playerMachinesContainer
	{
		get;
		set;
	}

	[Inject]
	private MachineClusterContainer machineClusterContainer
	{
		get;
		set;
	}

	[Inject]
	private MachineRootContainer machineRootContainer
	{
		get;
		set;
	}

	[Inject]
	private RigidbodyDataContainer rigidbodyDataContainer
	{
		get;
		set;
	}

	[Inject]
	private LivePlayersContainer livePlayersContainer
	{
		get;
		set;
	}

	[Inject]
	private PlayerTeamsContainer playerTeamsContainer
	{
		get;
		set;
	}

	[Inject]
	private NetworkMachineManager machineManager
	{
		get;
		set;
	}

	[Inject]
	private TeamBaseDestructionAudioTrigger destroyedCubeAudioTrigger
	{
		get;
		set;
	}

	[Inject]
	private MachineTeamColourUtility machineTeamColourUtility
	{
		get;
		set;
	}

	public PreloadedMachine GetPreloadedTeamBase(int id)
	{
		return _teamBases[id];
	}

	public PreloadedMachine GetPreloadedEqualizer()
	{
		return _equalizer;
	}

	public IEnumerator PreloadAllAsync()
	{
		ILoadBattleArenaSettingsRequest settingsRequest = serviceRequestFactory.Create<ILoadBattleArenaSettingsRequest>();
		TaskService<BattleArenaSettingsDependency> settingsTask = new TaskService<BattleArenaSettingsDependency>(settingsRequest);
		yield return settingsTask;
		BattleArenaSettingsDependency battleArenaSettings = settingsTask.result;
		ParallelTaskCollection c = new ParallelTaskCollection();
		c.Add(BuildTeamBases(battleArenaSettings));
		c.Add(BuildEqualizer(battleArenaSettings));
		c.Add(BuildCapturePoints());
		yield return c;
	}

	private IEnumerator BuildCapturePoints()
	{
		BuildCapturePointsCommand cmd = commandFactory.Build<BuildCapturePointsCommand>();
		cmd.Execute();
		yield return null;
	}

	private IEnumerator BuildTeamBases(BattleArenaSettingsDependency dependency)
	{
		for (int i = 0; i < 2; i++)
		{
			Console.Log("building base for " + i);
			PreloadedMachine preloadedBase = new PreloadedMachine();
			yield return PreloadMachine(preloadedBase, i, dependency.teamBaseModel, TargetType.TeamBase, GameLayers.TEAM_BASE);
			preloadedBase.machineBoard.set_name("TeamBase_" + i);
			_teamBases.Add(i, preloadedBase);
			yield return null;
			BuildTeamBase(i, dependency.protoniumHealth);
			yield return null;
		}
	}

	private IEnumerator BuildEqualizer(BattleArenaSettingsDependency dependency)
	{
		int entityId = 0;
		Console.Log("building equalizer");
		_equalizer = new PreloadedMachine();
		yield return PreloadMachine(_equalizer, entityId, dependency.equalizerModel, TargetType.EqualizerCrystal, GameLayers.EQUALIZER);
		_equalizer.machineBoard.set_name("Equalizer_0");
		int team = -1;
		PreloadedMachine preloadedMachine = _equalizer;
		EqualizerImplementor implementor = preloadedMachine.machineBoard.GetComponentInChildren<EqualizerImplementor>();
		implementor.machineId = 0;
		implementor.playerId = 0;
		implementor.ownerTeamId = team;
		implementor.visualTeam = VisualTeam.None;
		implementor.rb = preloadedMachine.rbData;
		implementor.root = preloadedMachine.machineBoard;
		implementor.machineMap = preloadedMachine.machineMap;
		SpotterStructureImplementor spotterStructure = preloadedMachine.machineBoard.GetComponentInChildren<SpotterStructureImplementor>();
		entityFactory.BuildEntity<EqualizerEntityDescriptor>(preloadedMachine.machineBoard.GetInstanceID(), new object[2]
		{
			implementor,
			spotterStructure
		});
		preloadedMachine.machineBoard.SetActive(true);
		int machineId = entityId;
		playerMachinesContainer.RegisterPlayerMachine(TargetType.EqualizerCrystal, entityId, machineId);
		machineClusterContainer.RegisterMachineCluster(TargetType.EqualizerCrystal, machineId, preloadedMachine.machineGraph.cluster);
		machineRootContainer.RegisterMachineRoot(TargetType.EqualizerCrystal, machineId, preloadedMachine.machineBoard);
		rigidbodyDataContainer.RegisterRigidBodyData(TargetType.EqualizerCrystal, machineId, preloadedMachine.rbData);
		machineManager.RegisterMachineMap(TargetType.EqualizerCrystal, machineId, preloadedMachine.machineMap);
		livePlayersContainer.MarkAsLive(TargetType.EqualizerCrystal, entityId);
		playerTeamsContainer.RegisterPlayerTeam(TargetType.EqualizerCrystal, entityId, team);
		yield return null;
	}

	private IEnumerator PreloadMachine(PreloadedMachine preloadedMachine, int id, byte[] modelData, TargetType type, int layer)
	{
		preloadedMachine.machineModel = new MachineModel(modelData);
		preloadedMachine.machineId = id;
		RBEntity rbentity = PhysicsActivator.ActivatePhysicsKinematic();
		preloadedMachine.rbData = rbentity.rigidBody;
		preloadedMachine.machineBoard = rbentity.board;
		preloadedMachine.machineBoard.SetActive(false);
		yield return machineSimulationBuilder.SetupSimulationEntity(preloadedMachine, type, layer);
	}

	private void BuildTeamBase(int teamId, int protoniumCubeHealth)
	{
		bool flag = IsMyTeam(teamId);
		PreloadedMachine preloadedMachine = _teamBases[teamId];
		preloadedMachine.machineBoard.SetActive(true);
		SetAllCubesHealth(preloadedMachine.machineMap.GetAllInstantiatedCubes(), protoniumCubeHealth);
		RegisterBaseData(preloadedMachine, teamId);
		GameObject claspGameObject = GetClaspGameObject(preloadedMachine.machineBoard);
		Transform transform = claspGameObject.get_transform();
		if (!flag)
		{
			TeamBaseClientImplementor teamBaseClientImplementor = new TeamBaseClientImplementor();
			teamBaseClientImplementor.visualTeam = VisualTeam.EnemyTeam;
			SpotterStructureImplementor componentInChildren = claspGameObject.GetComponentInChildren<SpotterStructureImplementor>();
			entityFactory.BuildEntity<TeamBaseClientEntityDescriptor>(claspGameObject.GetInstanceID(), new object[2]
			{
				componentInChildren,
				teamBaseClientImplementor
			});
		}
		SetBaseColour(flag, preloadedMachine);
		SetBeamColour(flag, transform);
		BuildFusionShield(flag, transform, teamId);
		destroyedCubeAudioTrigger.AddBase(teamId, claspGameObject);
	}

	public static GameObject GetClaspGameObject(GameObject board)
	{
		return board.GetComponentInChildren<CubeInstance>().get_gameObject();
	}

	private void SetAllCubesHealth(FasterList<InstantiatedCube> allCubes, int health)
	{
		for (int i = 0; i < allCubes.get_Count(); i++)
		{
			allCubes.get_Item(i).initialTotalHealth = health;
		}
	}

	private void RegisterBaseData(PreloadedMachine preloadedMachine, int team)
	{
		int machineId = preloadedMachine.machineId = team;
		playerMachinesContainer.RegisterPlayerMachine(TargetType.TeamBase, team, machineId);
		machineClusterContainer.RegisterMachineCluster(TargetType.TeamBase, machineId, preloadedMachine.machineGraph.cluster);
		machineRootContainer.RegisterMachineRoot(TargetType.TeamBase, machineId, preloadedMachine.machineBoard);
		rigidbodyDataContainer.RegisterRigidBodyData(TargetType.TeamBase, machineId, preloadedMachine.rbData);
		machineManager.RegisterMachineMap(TargetType.TeamBase, machineId, preloadedMachine.machineMap);
		livePlayersContainer.MarkAsLive(TargetType.TeamBase, team);
		playerTeamsContainer.RegisterPlayerTeam(TargetType.TeamBase, team, team);
	}

	private void BuildFusionShield(bool isAlly, Transform drivingSeat, int teamId)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bool powerState = false;
		GameObject val = gameObjectFactory.Build((!isAlly) ? "FusionShield_Red" : "FusionShield_Blue");
		val.get_transform().SetParent(drivingSeat);
		val.get_transform().set_localPosition(Vector3.get_zero());
		val.get_transform().set_localRotation(Quaternion.get_identity());
		IFusionShieldActivable component = val.GetComponent<IFusionShieldActivable>();
		component.powerState = powerState;
		FusionShieldImplementor component2 = val.GetComponent<FusionShieldImplementor>();
		component2.ownerTeamId = teamId;
		component2.InitializeColliders();
		entityFactory.BuildEntity<FusionShieldEntityDescriptor>(teamId, new object[2]
		{
			component,
			component2
		});
	}

	private void SetBaseColour(bool myTeam, PreloadedMachine preloadedMachine)
	{
		machineTeamColourUtility.SetRobotTeamColors(!myTeam, preloadedMachine.machineBoard);
	}

	private void SetBeamColour(bool myTeam, Transform baseCube)
	{
		BaseBeamTeamSwitcher component = baseCube.GetComponent<BaseBeamTeamSwitcher>();
		component.SwitchBeam(myTeam);
	}

	private bool IsMyTeam(int team)
	{
		return playerTeamsContainer.IsMyTeam(team);
	}
}
