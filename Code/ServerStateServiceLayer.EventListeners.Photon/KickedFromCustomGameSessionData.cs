namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class KickedFromCustomGameSessionData
	{
		public readonly string SessionID;

		public readonly bool WasInvited;

		public KickedFromCustomGameSessionData(string sessionID_, bool wasinvited_)
		{
			SessionID = sessionID_;
			WasInvited = wasinvited_;
		}
	}
}
