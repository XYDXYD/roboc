namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameRobotTierChangedEventData
	{
		public readonly string SessionID;

		public readonly string TargetUsername;

		public readonly int NewTier;

		public CustomGameRobotTierChangedEventData(string sessionID_, string targetUserName_, int newTier_)
		{
			SessionID = sessionID_;
			TargetUsername = targetUserName_;
			NewTier = newTier_;
		}
	}
}
