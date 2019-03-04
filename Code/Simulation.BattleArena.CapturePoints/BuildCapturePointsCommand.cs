using Battle;
using Simulation.BattleArena.CapturePoint;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Svelto.Command;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.BattleArena.CapturePoints
{
	internal class BuildCapturePointsCommand : ICommand
	{
		private const float CAPTURE_POINT_RADIUS = 15.8f;

		[Inject]
		private IGameObjectFactory gameObjectFactory
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
		private PlayerTeamsContainer playerTeamsContainer
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
		private BattlePlayers battlePlayers
		{
			get;
			set;
		}

		public void Execute()
		{
			BuildCapturePoints();
		}

		private void BuildCapturePoints()
		{
			BuildCapturePoint(CapturePointId.one);
			BuildCapturePoint(CapturePointId.two);
			BuildCapturePoint(CapturePointId.three);
		}

		private void BuildCapturePoint(CapturePointId position)
		{
			int num = -1;
			GameObject val = gameObjectFactory.Build("BA_CapturePoint");
			CapturePointImplementor capturePointImplementor = new CapturePointImplementor();
			capturePointImplementor.visualPositionId = GetCapturePointVisualId((int)position);
			capturePointImplementor.root = val;
			capturePointImplementor.squareRadius = 249.64f;
			capturePointImplementor.state = CaptureState.none;
			capturePointImplementor.visualTeam = VisualTeam.None;
			capturePointImplementor.ownerTeamId = num;
			capturePointImplementor.progressPercent = 0f;
			capturePointImplementor.maxProgress = 0f;
			CapturePointMonoBehaviour component = val.GetComponent<CapturePointMonoBehaviour>();
			SpotterStructureImplementor component2 = val.GetComponent<SpotterStructureImplementor>();
			entityFactory.BuildEntity<CapturePointEntityDescriptor>((int)position, new object[3]
			{
				capturePointImplementor,
				component,
				component2
			});
			val.SetActive(true);
			playerTeamsContainer.RegisterPlayerTeam(TargetType.CapturePoint, (int)position, num);
			machineRootContainer.RegisterMachineRoot(TargetType.CapturePoint, (int)position, val);
		}

		private CapturePointVisualPosition GetCapturePointVisualId(int id)
		{
			if (id == 0 && battlePlayers.MyTeam == 1)
			{
				return CapturePointVisualPosition.Far;
			}
			if (id == 2 && battlePlayers.MyTeam == 1)
			{
				return CapturePointVisualPosition.Near;
			}
			return (CapturePointVisualPosition)id;
		}
	}
}
