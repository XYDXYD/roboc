using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityViews
{
	internal class CurrentWaveWidgetEntityView : EntityView
	{
		public IWidgetCounterComponent CurrentWaveWidgetCounterComponent;

		public CurrentWaveWidgetEntityView()
			: this()
		{
		}
	}
}
