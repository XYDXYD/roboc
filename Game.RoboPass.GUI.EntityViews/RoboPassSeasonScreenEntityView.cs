using Game.ECS.GUI.Components;
using Game.RoboPass.GUI.Components;
using Svelto.ECS;

namespace Game.RoboPass.GUI.EntityViews
{
	internal class RoboPassSeasonScreenEntityView : EntityView
	{
		public IButtonComponent buttonComponent;

		public IRewardsGridsComponent rewardsGridsComponent;

		public IRootGOComponent rootGOComponent;

		public RoboPassSeasonScreenEntityView()
			: this()
		{
		}
	}
}
