using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal sealed class LoadShopEarningsRequest : WebServicesRequest<LoadShopEarningsResponse>, ILoadShopEarningsRequest, IServiceRequest, IAnswerOnComplete<LoadShopEarningsResponse>
	{
		protected override byte OperationCode => 88;

		public LoadShopEarningsRequest()
			: base("strRobotShopError", "strErrorGetCommunityShopEarnings", 0)
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

		protected override LoadShopEarningsResponse ProcessResponse(OperationResponse response)
		{
			Hashtable val = response.Parameters[96] as Hashtable;
			LoadShopEarningsResponse loadShopEarningsResponse = new LoadShopEarningsResponse();
			loadShopEarningsResponse.earnCount = (int)val.get_Item((object)"buyCount");
			loadShopEarningsResponse.earnings = (int)val.get_Item((object)"earnings");
			return loadShopEarningsResponse;
		}
	}
}
