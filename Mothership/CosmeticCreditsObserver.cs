using Svelto.Observer;
using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class CosmeticCreditsObserver : Observer<long>
	{
		public CosmeticCreditsObserver(Observable<long> observable)
			: base(observable)
		{
		}
	}
}
