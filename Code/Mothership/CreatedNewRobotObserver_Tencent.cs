using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class CreatedNewRobotObserver_Tencent : Observer<CreateNewRobotDependency>
	{
		public CreatedNewRobotObserver_Tencent(CreatedNewRobotObservable_Tencent observable)
			: base(observable)
		{
		}
	}
}
