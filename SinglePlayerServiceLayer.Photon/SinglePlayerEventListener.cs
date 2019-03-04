namespace SinglePlayerServiceLayer.Photon
{
	internal abstract class SinglePlayerEventListener<T, U> : PhotonEventListener<T, U>
	{
		public abstract SinglePlayerEventCode SinglePlayerEventCode
		{
			get;
		}

		public override int EventCode => (int)SinglePlayerEventCode;
	}
	internal abstract class SinglePlayerEventListener<T> : PhotonEventListener<T>
	{
		public abstract SinglePlayerEventCode SinglePlayerEventCode
		{
			get;
		}

		public override int EventCode => (int)SinglePlayerEventCode;
	}
	internal abstract class SinglePlayerEventListener : PhotonEventListener
	{
		public abstract SinglePlayerEventCode SinglePlayerEventCode
		{
			get;
		}

		public override int EventCode => (int)SinglePlayerEventCode;
	}
}
