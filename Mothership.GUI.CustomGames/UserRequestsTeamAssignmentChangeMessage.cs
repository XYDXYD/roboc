namespace Mothership.GUI.CustomGames
{
	internal class UserRequestsTeamAssignmentChangeMessage
	{
		public readonly int SourceSlotIndex;

		public readonly int DestinationSlotIndex;

		public readonly bool MovingFromTeamA;

		public UserRequestsTeamAssignmentChangeMessage(int sourceSlotIndex_, int destinationSlotIndex_, bool movingFromTeamA_)
		{
			SourceSlotIndex = sourceSlotIndex_;
			DestinationSlotIndex = destinationSlotIndex_;
			MovingFromTeamA = movingFromTeamA_;
		}
	}
}
