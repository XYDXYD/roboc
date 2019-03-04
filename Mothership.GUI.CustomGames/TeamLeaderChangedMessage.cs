namespace Mothership.GUI.CustomGames
{
	internal class TeamLeaderChangedMessage
	{
		public readonly bool IsLocalPlayerTeamLeader;

		public TeamLeaderChangedMessage(bool isLocalPlayerTeamLeader_)
		{
			IsLocalPlayerTeamLeader = isLocalPlayerTeamLeader_;
		}
	}
}
