namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class DeclineInviteToSessionData
	{
		public readonly string PlayerWhoDeclined;

		public DeclineInviteToSessionData(string playerWhoDeclined_)
		{
			PlayerWhoDeclined = playerWhoDeclined_;
		}
	}
}
