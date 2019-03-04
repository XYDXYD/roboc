using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityViews
{
	internal class WaveGoalsWidgetShowEntityView : EntityView
	{
		public IEliminationComponent eliminationComponent;

		public ITimedEliminationComponent timedEliminationComponent;

		public ISurvivalComponent survivalComponent;

		public WaveGoalsWidgetShowEntityView()
			: this()
		{
		}
	}
}
