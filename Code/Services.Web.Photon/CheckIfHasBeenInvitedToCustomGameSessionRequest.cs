using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CheckIfHasBeenInvitedToCustomGameSessionRequest : WebServicesCachedRequest<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse>, ICheckIfHasBeenInvitedToCustomGameSessionRequest, IServiceRequest, IAnswerOnComplete<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse>
	{
		protected override byte OperationCode => 0;

		public CheckIfHasBeenInvitedToCustomGameSessionRequest()
			: base("strCustomGameError", "strCustomGameFetchHasInvitationToCustomGameError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override CheckIfHasBeenInvitedToCustomGameSessionRequestResponse ProcessResponse(OperationResponse response)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			CheckIfHasBeenInvitedToCustomGameResponseCode checkIfHasBeenInvitedToCustomGameResponseCode = (CheckIfHasBeenInvitedToCustomGameResponseCode)response.get_Item((byte)168);
			if (checkIfHasBeenInvitedToCustomGameResponseCode == CheckIfHasBeenInvitedToCustomGameResponseCode.HasPendingCustomGameSessionInvitation)
			{
				CheckIfHasBeenInvitedToCustomGameSessionResponseData checkIfHasBeenInvitedToCustomGameSessionResponseData = new CheckIfHasBeenInvitedToCustomGameSessionResponseData();
				checkIfHasBeenInvitedToCustomGameSessionResponseData.Deserialise(response.get_Item((byte)189));
				return new CheckIfHasBeenInvitedToCustomGameSessionRequestResponse(checkIfHasBeenInvitedToCustomGameResponseCode, checkIfHasBeenInvitedToCustomGameSessionResponseData);
			}
			return new CheckIfHasBeenInvitedToCustomGameSessionRequestResponse(checkIfHasBeenInvitedToCustomGameResponseCode, null);
		}

		void ICheckIfHasBeenInvitedToCustomGameSessionRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
