using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal sealed class RobotDimensionChangedObserver : Observer<RobotDimensionDependency>
	{
		public RobotDimensionChangedObserver(RobotDimensionChangedObservable observable)
			: base(observable)
		{
		}
	}
}
