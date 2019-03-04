namespace LobbyServiceLayer.Photon
{
	internal abstract class LobbyEventListener<T, U> : PhotonEventListener<T, U>
	{
		public abstract LobbyEventCode LobbyEventCode
		{
			get;
		}

		public override int EventCode => (int)LobbyEventCode;
	}
	internal abstract class LobbyEventListener<T> : PhotonEventListener<T>
	{
		public abstract LobbyEventCode LobbyEventCode
		{
			get;
		}

		public override int EventCode => (int)LobbyEventCode;
	}
	internal abstract class LobbyEventListener : PhotonEventListener
	{
		public abstract LobbyEventCode LobbyEventCode
		{
			get;
		}

		public override int EventCode => (int)LobbyEventCode;
	}
}
