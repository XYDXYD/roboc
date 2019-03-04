using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class DeclineAllClanInvitesRequest : SocialRequest, IDeclineAllClanInvitesRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 44;

		public DeclineAllClanInvitesRequest()
			: base("strDeclineClanInviteErrorTitle", "strDeclineClanInviteErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override void SuccessfulResponse(OperationResponse response)
		{
			CacheDTO.ClanInvites = new Dictionary<string, ClanInvite>();
			base.SuccessfulResponse(response);
		}
	}
}
