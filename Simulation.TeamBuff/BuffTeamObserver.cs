using Svelto.Observer.IntraNamespace;

namespace Simulation.TeamBuff
{
	internal class BuffTeamObserver : Observer<TeamBuffDependency>
	{
		public BuffTeamObserver(BuffTeamObservable observable)
			: base(observable)
		{
		}
	}
}
