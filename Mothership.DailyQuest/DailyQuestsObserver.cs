using Svelto.Observer.IntraNamespace;

namespace Mothership.DailyQuest
{
	internal class DailyQuestsObserver : Observer<PlayerDailyQuestsData>
	{
		public DailyQuestsObserver(DailyQuestsObservable observable)
			: base(observable)
		{
		}
	}
}
