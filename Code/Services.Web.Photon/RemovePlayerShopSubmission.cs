using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class RemovePlayerShopSubmission : WebServicesRequest<LoadShopEarningsResponse>, IRemovePlayerShopSubmission, IServiceRequest<int>, IAnswerOnComplete<LoadShopEarningsResponse>, IServiceRequest
	{
		private int robotId;

		protected override byte OperationCode => 91;

		public RemovePlayerShopSubmission()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		public void Inject(int _robotId)
		{
			robotId = _robotId;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = robotId;
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
