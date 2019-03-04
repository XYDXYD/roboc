using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal sealed class ValidateCampaignRobotRequest : WebServicesRequest, IValidateCampaignRobotRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _campaignID = string.Empty;

		protected override byte OperationCode => 59;

		public ValidateCampaignRobotRequest()
			: base("strRobocloudError", "strUnableToValidatePlayerRobotForCampaigns", 0)
		{
		}

		public void Inject(string campaignID)
		{
			_campaignID = campaignID;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			OperationRequest val2 = val;
			val2.Parameters[22] = _campaignID;
			return val2;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
