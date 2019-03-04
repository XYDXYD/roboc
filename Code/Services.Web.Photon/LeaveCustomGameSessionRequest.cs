using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LeaveCustomGameSessionRequest : WebServicesCachedRequest<SessionLeaveResponseCode>, ILeaveCustomGameSessionRequest, IServiceRequest, IAnswerOnComplete<SessionLeaveResponseCode>
	{
		protected override byte OperationCode => 145;

		public LeaveCustomGameSessionRequest()
			: base("strCustomGameError", "strCustomGameSessionLeaveFail", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override SessionLeaveResponseCode ProcessResponse(OperationResponse response)
		{
			return (SessionLeaveResponseCode)response.get_Item((byte)168);
		}

		void ILeaveCustomGameSessionRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
