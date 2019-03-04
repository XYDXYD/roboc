internal class SetOwnPartyStatusMessage
{
	internal readonly bool WeAreLeader;

	internal readonly bool WeAreInQueue;

	internal SetOwnPartyStatusMessage(bool isLeader_, bool isInQueue_)
	{
		WeAreLeader = isLeader_;
		WeAreInQueue = isInQueue_;
	}
}
