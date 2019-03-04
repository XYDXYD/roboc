using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class DailyQuestChangedObserver : Observer<int>
	{
		public DailyQuestChangedObserver(DailyQuestChangedObservable observable)
			: base(observable)
		{
		}
	}
}
