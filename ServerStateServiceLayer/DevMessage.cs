namespace ServerStateServiceLayer
{
	internal class DevMessage
	{
		public readonly string Text;

		public readonly int DisplayTime;

		public DevMessage(string text_, int displayTime_)
		{
			Text = text_;
			DisplayTime = displayTime_;
		}
	}
}
