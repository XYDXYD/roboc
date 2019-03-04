using CustomGames;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class RetrieveCustomGameSessionRequest : WebServicesCachedRequest<RetrieveCustomGameSessionRequestData>, IRetrieveCustomGameSessionRequest, IServiceRequest, IAnswerOnComplete<RetrieveCustomGameSessionRequestData>
	{
		protected override byte OperationCode => 144;

		public RetrieveCustomGameSessionRequest()
			: base("strCustomGameError", "strCustomGameSessionRetrieveFail", 0)
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

		protected override RetrieveCustomGameSessionRequestData ProcessResponse(OperationResponse response)
		{
			CustomGameSessionRetrieveResponse customGameSessionRetrieveResponse = (CustomGameSessionRetrieveResponse)response.Parameters[168];
			if (customGameSessionRetrieveResponse != CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				return new RetrieveCustomGameSessionRequestData(null, customGameSessionRetrieveResponse);
			}
			object sourceData = response.Parameters[169];
			CustomGameSessionData sessionData_ = new CustomGameSessionData(sourceData);
			return new RetrieveCustomGameSessionRequestData(sessionData_, customGameSessionRetrieveResponse);
		}

		void IRetrieveCustomGameSessionRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
