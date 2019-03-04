using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace Services.TechTree
{
	internal class GetTechTreePointsAwardRequest : WebServicesRequest<int>, IGetTechPointsAwardRequest, IServiceRequest, IAnswerOnComplete<int>
	{
		protected override byte OperationCode => 185;

		public GetTechTreePointsAwardRequest()
			: base("strRobocloudError", "strUnableToGetTechTreeData", 3)
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

		protected override int ProcessResponse(OperationResponse response)
		{
			return (int)response.Parameters[212];
		}
	}
}
