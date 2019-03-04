using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class RespondToCustomGameInvitationRequest : WebServicesCachedRequest<ReplyToCustomGameInviteResponseCode>, IRespondToCustomGameInvitationRequest, IServiceRequest<RespondToCustomGameInvitationDependancy>, IAnswerOnComplete<ReplyToCustomGameInviteResponseCode>, IServiceRequest
	{
		private RespondToCustomGameInvitationDependancy _dependancy;

		protected override byte OperationCode => 148;

		public RespondToCustomGameInvitationRequest()
			: base("strCustomGameError", "strCustomGameRespondToInvitationError", 0)
		{
		}

		public void Inject(RespondToCustomGameInvitationDependancy dependency)
		{
			_dependancy = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[173] = _dependancy.IsAcceptInvite;
			val.Parameters = dictionary;
			return val;
		}

		protected override ReplyToCustomGameInviteResponseCode ProcessResponse(OperationResponse response)
		{
			return (ReplyToCustomGameInviteResponseCode)response.get_Item((byte)168);
		}

		void IRespondToCustomGameInvitationRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
