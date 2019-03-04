using Svelto.ECS;

namespace Mothership.GUI
{
	internal sealed class StatsHintPopupEntityView : EntityView
	{
		public IStatsHintComponent statsComponent;

		public IHintPopupComponent popupComponent;

		public StatsHintPopupEntityView()
			: this()
		{
		}
	}
}
