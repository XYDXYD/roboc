namespace Services.Web.Photon
{
	internal struct CheckIfHasBeenInvitedToCustomGameSessionRequestResponse
	{
		public CheckIfHasBeenInvitedToCustomGameResponseCode ResponseCode;

		public CheckIfHasBeenInvitedToCustomGameSessionResponseData ResponseData;

		public CheckIfHasBeenInvitedToCustomGameSessionRequestResponse(CheckIfHasBeenInvitedToCustomGameResponseCode response_, CheckIfHasBeenInvitedToCustomGameSessionResponseData data_)
		{
			ResponseCode = response_;
			ResponseData = data_;
		}
	}
}
