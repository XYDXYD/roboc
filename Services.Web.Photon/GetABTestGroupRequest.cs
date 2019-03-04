using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class GetABTestGroupRequest : WebServicesRequest<ABTestData>, IGetABTestGroupRequest, IServiceRequest, IAnswerOnComplete<ABTestData>
	{
		protected override byte OperationCode => 55;

		public GetABTestGroupRequest()
			: base("strRobocloudError", "strUnableGetABTestGroupRequest", 0)
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

		protected override ABTestData ProcessResponse(OperationResponse response)
		{
			ABTestData result = default(ABTestData);
			result.ABTest = (string)response.Parameters[166];
			result.ABTestGroup = (string)response.Parameters[167];
			return result;
		}
	}
}
