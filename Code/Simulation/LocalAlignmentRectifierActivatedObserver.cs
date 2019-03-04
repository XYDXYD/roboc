using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class LocalAlignmentRectifierActivatedObserver : Observer<bool>
	{
		public LocalAlignmentRectifierActivatedObserver(LocalAlignmentRectifierActivatedObservable activatedObservable)
			: base(activatedObservable)
		{
		}
	}
}
