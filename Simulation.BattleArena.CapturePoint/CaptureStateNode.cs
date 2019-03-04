using Svelto.ECS;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class CaptureStateNode : EntityView
	{
		public IRootComponent rootComponent;

		public IRangeComponent rangeComponent;

		public IStateComponent stateComponent;

		public IVisualTeamComponent visualTeamComponent;

		public CaptureStateNode()
			: this()
		{
		}
	}
}
