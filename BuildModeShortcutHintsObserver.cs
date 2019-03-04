using Svelto.Observer.IntraNamespace;

public sealed class BuildModeShortcutHintsObserver : Observer<BuildModeHintEvent>
{
	public BuildModeShortcutHintsObserver(BuildModeShortcutHintsObserveable observable)
		: base(observable)
	{
	}
}
