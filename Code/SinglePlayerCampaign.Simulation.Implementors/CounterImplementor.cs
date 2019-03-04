using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class CounterImplementor : ICounterComponent
	{
		public DispatchOnSet<int> counterValue
		{
			get;
			private set;
		}

		public int maxValue
		{
			get;
			set;
		}

		public CounterImplementor(int entityID, int value_, int maxValue_)
		{
			DispatchOnSet<int> val = new DispatchOnSet<int>(entityID);
			val.set_value(value_);
			counterValue = val;
			maxValue = maxValue_;
		}
	}
}
