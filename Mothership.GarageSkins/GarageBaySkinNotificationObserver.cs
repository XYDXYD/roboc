using Svelto.Observer.IntraNamespace;

namespace Mothership.GarageSkins
{
	internal sealed class GarageBaySkinNotificationObserver : Observer<bool>
	{
		public GarageBaySkinNotificationObserver(GarageBaySkinNotificationObservable observable)
			: base(observable)
		{
		}
	}
}
