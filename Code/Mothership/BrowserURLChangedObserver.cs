using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class BrowserURLChangedObserver : Observer<string>
	{
		public BrowserURLChangedObserver(BrowserURLChangedObservable observable)
			: base(observable)
		{
		}
	}
}
