namespace Mothership
{
	internal enum PlatoonQueueState
	{
		NoParty,
		MemberReadyUp,
		MemberPartyWaitingForYou,
		LeaderReadyUp,
		LeaderPartyWaitingForYou,
		QueuingWaitingOnParty,
		AllPlayersQueued
	}
}
