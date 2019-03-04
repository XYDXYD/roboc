using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class ChangeTeamAssignmentRequest : WebServicesRequest<ChangeCustomGameTeamAssignmentResponse>, IChangeTeamAssignmentRequest, IServiceRequest<ChangeCustomGameTeamAssignmentDependancy>, IAnswerOnComplete<ChangeCustomGameTeamAssignmentResponse>, IServiceRequest
	{
		private ChangeCustomGameTeamAssignmentDependancy _dependancy;

		protected override byte OperationCode => 151;

		public ChangeTeamAssignmentRequest()
			: base("strCustomGameError", "strCustomGameAdjustTeamsError", 0)
		{
		}

		public void Inject(ChangeCustomGameTeamAssignmentDependancy dependency)
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
			dictionary[185] = _dependancy.sourcePlayer;
			dictionary[186] = _dependancy.TargetPlayer;
			dictionary[187] = _dependancy.DestinationIsTeamB;
			val.Parameters = dictionary;
			return val;
		}

		protected override ChangeCustomGameTeamAssignmentResponse ProcessResponse(OperationResponse response)
		{
			return (ChangeCustomGameTeamAssignmentResponse)response.get_Item((byte)168);
		}
	}
}
