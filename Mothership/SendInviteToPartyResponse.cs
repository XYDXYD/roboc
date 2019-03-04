namespace Mothership
{
	internal class SendInviteToPartyResponse
	{
		public bool success
		{
			get;
			private set;
		}

		public string errorMsg
		{
			get;
			private set;
		}

		public SendInviteToPartyResponse()
		{
			success = true;
		}

		public SendInviteToPartyResponse(string errorMsg)
		{
			success = false;
			this.errorMsg = errorMsg;
		}
	}
}
