using Svelto.Observer.IntraNamespace;

internal sealed class TutorialTipObserver : Observer<TutorialMessageData>
{
	public TutorialTipObserver(TutorialTipObservable observable)
		: base(observable)
	{
	}
}
