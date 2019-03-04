using Game.RoboPass.Components;
using Svelto.ECS;

namespace Game.RoboPass.EntityViews
{
	internal class RoboPassSeasonDataEntityView : EntityView
	{
		public IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent;

		public IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent;

		public RoboPassSeasonDataEntityView()
			: this()
		{
		}
	}
}
