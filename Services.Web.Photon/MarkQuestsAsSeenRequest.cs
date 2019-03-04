using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class MarkQuestsAsSeenRequest : WebServicesRequest, IMarkQuestsAsSeenRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 36;

		public MarkQuestsAsSeenRequest()
			: base("strRobocloudError", "strErrorMarkQuestsAsSeen", 0)
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
