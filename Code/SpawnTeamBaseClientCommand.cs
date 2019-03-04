using Battle;
using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

internal sealed class SpawnTeamBaseClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private GetTeamBaseDependency _dependency;

	[Inject]
	public IMinimapPresenter minimapPresenter
	{
		private get;
		set;
	}

	[Inject]
	public MachineSpawnDispatcher spawnDispatcher
	{
		private get;
		set;
	}

	[Inject]
	public TeamBasePreloader teamBasePreloader
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
	public BattlePlayers battlePlayers
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as GetTeamBaseDependency);
		return this;
	}

	public void Execute()
	{
		SpawnBases();
	}

	private void SpawnBases()
	{
		SpawnTeamBase(0, _dependency.protoniumCubeHealth);
		SpawnTeamBase(1, _dependency.protoniumCubeHealth);
	}

	private void SpawnTeamBase(int teamId, int protoniumCubeHealth)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = IsMyTeam(teamId);
		PreloadedMachine preloadedTeamBase = teamBasePreloader.GetPreloadedTeamBase(teamId);
		preloadedTeamBase.machineBoard.SetActive(true);
		GameObject claspGameObject = TeamBasePreloader.GetClaspGameObject(preloadedTeamBase.machineBoard);
		Transform transform = claspGameObject.get_transform();
		ApplyRigidbodyTransform(preloadedTeamBase, _dependency.positions[teamId], _dependency.rotations[teamId], transform);
		minimapPresenter.RegisterBasePosition(_dependency.positions[teamId], flag);
		BattleArenaExtraData battleArenaExtraData = new BattleArenaExtraData();
		battleArenaExtraData.protoniumHealth = protoniumCubeHealth;
		SpawnInParametersEntity spawnInParameters = new SpawnInParametersEntity(teamId, teamId, flag, TargetType.TeamBase, preloadedTeamBase, battleArenaExtraData);
		spawnDispatcher.EntitySpawnedIn(spawnInParameters);
	}

	private void ApplyRigidbodyTransform(PreloadedMachine preloadedMachine, Vector3 position, Quaternion rotation, Transform claspTransform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position2 = position - rotation * claspTransform.get_localPosition();
		Transform transform = preloadedMachine.machineBoard.get_transform();
		transform.set_position(position2);
		transform.set_rotation(rotation);
		Vector3 position3 = preloadedMachine.rbData.get_position();
		position3.y -= 1.9f;
		preloadedMachine.rbData.set_position(position3);
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
		return battlePlayers.MyTeam == (uint)team;
	}
}
