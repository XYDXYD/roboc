namespace Mothership
{
	public class SocialMessage
	{
		public SocialMessageType messageDispatched;

		public string extraDetails;

		public object extraData;

		public SocialMessage(SocialMessageType messageType_, string extraDetails_ = "", object extraData_ = null)
		{
			messageDispatched = messageType_;
			extraDetails = extraDetails_;
			extraData = extraData_;
		}
	}
}
