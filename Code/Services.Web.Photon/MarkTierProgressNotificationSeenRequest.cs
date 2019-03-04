using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class MarkTierProgressNotificationSeenRequest : WebServicesRequest, IMarkTierProgressNotificationSeenRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 109;

		public MarkTierProgressNotificationSeenRequest()
			: base("strGenericError", "strFlagLeagueSeenRequestFailed", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
