using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace Services.TechTree
{
	internal class MarkTechPointsAwardAsSeenRequest : WebServicesRequest, IMarkTechPointsAwardAsSeenRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 186;

		public MarkTechPointsAwardAsSeenRequest()
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

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
