namespace SinglePlayerCampaign.GUI.Simulation.Components
{
	public interface IWidgetCounterComponent
	{
		int WidgetCounterValue
		{
			set;
		}

		int WidgetCounterMaxValue
		{
			get;
			set;
		}
	}
}
