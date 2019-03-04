namespace Login
{
	public class SplashLoginGUIMessage
	{
		public SplashLoginGUIMessageType Message;

		public SplashLoginGUIMessage(SplashLoginGUIMessageType messageType_, object extraData_ = null)
		{
			Message = messageType_;
		}
	}
}
