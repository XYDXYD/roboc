using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	public interface ICounterComponent
	{
		DispatchOnSet<int> counterValue
		{
			get;
		}

		int maxValue
		{
			get;
			set;
		}
	}
}
