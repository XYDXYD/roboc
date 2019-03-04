using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class UpdateVotingAfterBattleClientCommandObserver : Observer<UpdateVotingAfterBattleDependency>
	{
		public UpdateVotingAfterBattleClientCommandObserver(UpdateVotingAfterBattleClientCommandObservable observable)
			: base(observable)
		{
		}
	}
}
