using Svelto.Observer.IntraNamespace;

internal sealed class InvalidPlacementObserver : Observer<InvalidPlacementType>
{
	public InvalidPlacementObserver(InvalidPlacementObservable observable)
		: base(observable)
	{
	}
}
