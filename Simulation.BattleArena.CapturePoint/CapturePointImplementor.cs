using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class CapturePointImplementor : IVisualPositionComponent, IRootComponent, IRangeComponent, IStateComponent, IVisualTeamComponent, IOwnerTeamComponent, IProgressComponent
	{
		public GameObject root
		{
			get;
			set;
		}

		public float squareRadius
		{
			get;
			set;
		}

		public CapturePointVisualPosition visualPositionId
		{
			get;
			set;
		}

		public CaptureState state
		{
			get;
			set;
		}

		public int ownerTeamId
		{
			get;
			set;
		}

		public VisualTeam visualTeam
		{
			get;
			set;
		}

		public float progressPercent
		{
			get;
			set;
		}

		public float maxProgress
		{
			get;
			set;
		}
	}
}
