using Svelto.Observer.IntraNamespace;

namespace Login
{
	internal sealed class IntroAnimationsSequenceEventObserver : Observer<IntroAnimationsSequenceEventCode>
	{
		public IntroAnimationsSequenceEventObserver(IntroAnimationsSequenceEventObservable observable)
			: base(observable)
		{
		}
	}
}
