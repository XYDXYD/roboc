using Svelto.Observer.IntraNamespace;

namespace Mothership.GarageSkins
{
	internal sealed class GarageBaySkinSelectedObserver : Observer<RobotBaySkinDependency>
	{
		public GarageBaySkinSelectedObserver(GarageBaySkinSelectedObservable observable)
			: base(observable)
		{
		}
	}
}
