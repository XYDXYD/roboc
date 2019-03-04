namespace ChatServiceLayer
{
	internal class CanSendWhisperRequestResult
	{
		public CanSandPrivateMessageResult canSendPrivateMessageResult
		{
			get;
			private set;
		}

		public string receiverName
		{
			get;
			private set;
		}

		public string displayName
		{
			get;
			private set;
		}

		public CanSendWhisperRequestResult(string receiverName, string displayName, CanSandPrivateMessageResult canSendPrivateMessageResult)
		{
			this.receiverName = receiverName;
			this.displayName = displayName;
			this.canSendPrivateMessageResult = canSendPrivateMessageResult;
		}
	}
}
