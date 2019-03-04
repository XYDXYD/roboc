using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class SetBuildModeHintsAnchorsObserver : Observer<BuildModeAnchors>
	{
		public SetBuildModeHintsAnchorsObserver(SetBuildModeHintsAnchorsObserverable observable)
			: base(observable)
		{
		}
	}
}
