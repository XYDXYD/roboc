namespace ChatServiceLayer.Photon
{
	internal abstract class ChatEventListener<T, U> : PhotonEventListener<T, U>
	{
		public abstract ChatEventCode ChatEventCode
		{
			get;
		}

		public override int EventCode => (int)ChatEventCode;
	}
	internal abstract class ChatEventListener<T> : PhotonEventListener<T>
	{
		public abstract ChatEventCode ChatEventCode
		{
			get;
		}

		public override int EventCode => (int)ChatEventCode;
	}
	internal abstract class ChatEventListener : PhotonEventListener
	{
		public abstract ChatEventCode ChatEventCode
		{
			get;
		}

		public override int EventCode => (int)ChatEventCode;
	}
}
