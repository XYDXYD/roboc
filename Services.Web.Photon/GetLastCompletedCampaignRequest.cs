using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class GetLastCompletedCampaignRequest : WebServicesRequest<LastCompletedCampaign?>, IGetLastCompletedCampaignRequest, IServiceRequest, IAnswerOnComplete<LastCompletedCampaign?>
	{
		protected override byte OperationCode => 77;

		public GetLastCompletedCampaignRequest()
			: base("strRobocloudError", "strUnableToGetLastCompletedCampaign", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override LastCompletedCampaign? ProcessResponse(OperationResponse response)
		{
			if (!(bool)response.get_Item((byte)89))
			{
				return null;
			}
			string campaignId_ = (string)response.get_Item((byte)22);
			int difficulty_ = (int)response.get_Item((byte)23);
			return new LastCompletedCampaign(campaignId_, difficulty_);
		}
	}
}
