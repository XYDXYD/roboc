using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class CubePrerequisitesFailedObserver : Observer<string>
	{
		public CubePrerequisitesFailedObserver(CubePrerequisitesFailedObservable observable)
			: base(observable)
		{
		}
	}
}
