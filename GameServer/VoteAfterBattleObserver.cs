using Svelto.Observer.IntraNamespace;

namespace GameServer
{
	internal class VoteAfterBattleObserver : Observer<UpdateVotingAfterBattleDependency>
	{
		public VoteAfterBattleObserver(VoteAfterBattleObservable observable)
			: base(observable)
		{
		}
	}
}
