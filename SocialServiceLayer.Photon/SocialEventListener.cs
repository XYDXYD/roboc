namespace SocialServiceLayer.Photon
{
	internal abstract class SocialEventListener<T, U> : PhotonEventListener<T, U>
	{
		public abstract SocialEventCode SocialEventCode
		{
			get;
		}

		public override int EventCode => (int)SocialEventCode;
	}
	internal abstract class SocialEventListener<T> : PhotonEventListener<T>
	{
		public abstract SocialEventCode SocialEventCode
		{
			get;
		}

		public override int EventCode => (int)SocialEventCode;
	}
	internal abstract class SocialEventListener : PhotonEventListener
	{
		public abstract SocialEventCode SocialEventCode
		{
			get;
		}

		public override int EventCode => (int)SocialEventCode;
	}
}
