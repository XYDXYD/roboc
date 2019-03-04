using Svelto.ECS;
using Tiers;

namespace Mothership.GUI
{
	internal sealed class RobotRankingWidgetsEntityView : EntityView
	{
		public IRobotRankingComponent RobotRankingComponent;

		public IRobotRankingWidgetComponent RobotRankingWidgetComponent;

		public RobotRankingWidgetsEntityView()
			: this()
		{
		}
	}
}
