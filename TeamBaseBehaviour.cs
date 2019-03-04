using Battle;
using Simulation;
using Simulation.Sight;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

public sealed class TeamBaseBehaviour : MonoBehaviour
{
	public int teamId;

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IMinimapPresenter minimapPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal BattlePlayers battlePlayers
	{
		private get;
		set;
	}

	[Inject]
	internal IEntityFactory entityFactory
	{
		private get;
		set;
	}

	public TeamBaseBehaviour()
		: this()
	{
	}

	public void Start()
	{
		if (GameModeRequiresBaseCapture())
		{
			InitialiseBase();
		}
	}

	private bool GameModeRequiresBaseCapture()
	{
		return WorldSwitching.GetGameModeType() == GameModeType.SuddenDeath;
	}

	private void InitialiseBase()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		int myTeam = (int)battlePlayers.MyTeam;
		GameObject val;
		if (myTeam == teamId)
		{
			val = gameObjectFactory.Build("CTF_BLUE_BASE");
		}
		else
		{
			val = gameObjectFactory.Build("CTF_RED_BASE 1");
			TeamBaseClientImplementor teamBaseClientImplementor = new TeamBaseClientImplementor();
			teamBaseClientImplementor.visualTeam = VisualTeam.EnemyTeam;
			SpotterStructureImplementor component = val.GetComponent<SpotterStructureImplementor>();
			entityFactory.BuildEntity<TeamBaseClientEntityDescriptor>(val.GetInstanceID(), new object[2]
			{
				component,
				teamBaseClientImplementor
			});
		}
		val.get_transform().set_parent(this.get_transform());
		val.get_transform().set_localPosition(Vector3.get_zero());
		val.get_transform().set_localRotation(Quaternion.get_identity());
		TeamBaseAnimation componentInChildren = val.GetComponentInChildren<TeamBaseAnimation>();
		componentInChildren.SetTeamId(teamId);
		TeamBaseAudioBehaviour componentInChildren2 = val.GetComponentInChildren<TeamBaseAudioBehaviour>();
		componentInChildren2.SetTeamBaseBehaviour(this);
		minimapPresenter.RegisterBasePosition(this.get_transform().get_position(), myTeam == teamId);
	}
}
