namespace Mothership.GUI.Party
{
	public class PartyIconInfo
	{
		public bool emptySlot;

		public bool isLeader;

		public bool robotTierMatchesLeaderTier;

		public bool isReadyState;

		public PartyIconInfo()
		{
			emptySlot = true;
			isLeader = false;
			robotTierMatchesLeaderTier = false;
			isReadyState = false;
		}

		public PartyIconInfo(bool emptySlot_, bool isLeader_, bool robotTierMatchesLeaderTier_, bool isready_)
		{
			emptySlot = emptySlot_;
			isLeader = isLeader_;
			robotTierMatchesLeaderTier = robotTierMatchesLeaderTier_;
			isReadyState = isready_;
		}
	}
}
