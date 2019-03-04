using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCRFItemListRequest : WebServicesRequest<LoadCrfItemListRequestResponse>, ILoadCRFItemListRequest, IServiceRequest<CRFShopItemListDependency>, IAnswerOnComplete<LoadCrfItemListRequestResponse>, IServiceRequest
	{
		private CRFShopItemListDependency _parameters;

		protected override byte OperationCode => 86;

		public LoadCRFItemListRequest()
			: base("strRobotShopError", "strNewRobotShopItemListError", 0)
		{
		}

		public void Inject(CRFShopItemListDependency parameters)
		{
			_parameters = new CRFShopItemListDependency(parameters);
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[92] = _parameters.Serialise();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override LoadCrfItemListRequestResponse ProcessResponse(OperationResponse response)
		{
			byte[] data = response.Parameters[93] as byte[];
			return new LoadCrfItemListRequestResponse(data);
		}
	}
}
