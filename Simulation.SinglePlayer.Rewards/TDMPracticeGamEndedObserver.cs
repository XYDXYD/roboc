using Svelto.Observer.IntraNamespace;

namespace Simulation.SinglePlayer.Rewards
{
	internal class TDMPracticeGamEndedObserver : Observer<int>
	{
		public TDMPracticeGamEndedObserver(TDMPracticeGamEndedObservable tdmPracticeGamEndedObservable)
			: base(tdmPracticeGamEndedObservable)
		{
		}
	}
}
