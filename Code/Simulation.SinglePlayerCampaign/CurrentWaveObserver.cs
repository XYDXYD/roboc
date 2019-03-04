using Simulation.SinglePlayerCampaign.DataTypes;
using Svelto.Observer.IntraNamespace;

namespace Simulation.SinglePlayerCampaign
{
	internal sealed class CurrentWaveObserver : Observer<WaveCounterInfo>
	{
		public CurrentWaveObserver(CurrentWaveObservable observable)
			: base(observable)
		{
		}
	}
}
