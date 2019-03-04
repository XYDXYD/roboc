using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class DispatchCustomGameInvitationRequest : WebServicesRequest<DispatchCustomGameInviteResponse>, IDispatchCustomGameInvitationRequest, IServiceRequest<DispatchCustomGameInviteDependancy>, IAnswerOnComplete<DispatchCustomGameInviteResponse>, IServiceRequest
	{
		private bool _isTeamA;

		private string _invitee;

		protected override byte OperationCode => 147;

		public DispatchCustomGameInvitationRequest()
			: base("strCustomGameError", "strCustomGameSessionDispatchFail", 0)
		{
		}

		public void Inject(DispatchCustomGameInviteDependancy dependancy)
		{
			_isTeamA = dependancy.IsTeamA;
			_invitee = dependancy.Invitee;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[171] = _invitee;
			dictionary[175] = _isTeamA;
			val.Parameters = dictionary;
			return val;
		}

		protected override DispatchCustomGameInviteResponse ProcessResponse(OperationResponse response)
		{
			return (DispatchCustomGameInviteResponse)response.Parameters[168];
		}
	}
}
