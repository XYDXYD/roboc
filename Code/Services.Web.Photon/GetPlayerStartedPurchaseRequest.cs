using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal sealed class GetPlayerStartedPurchaseRequest : WebServicesRequest<bool>, IGetPlayerStartedPurchaseRequest, IServiceRequest, IAnswerOnComplete<bool>
	{
		protected override byte OperationCode => 54;

		public GetPlayerStartedPurchaseRequest()
			: base("strRobocloudError", "strUnableToGetPlayerStartedPurchaseRequest", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			return (bool)response.Parameters[135];
		}
	}
}
