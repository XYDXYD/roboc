using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CreateCustomGameSessionRequest : WebServicesCachedRequest<SessionCreationResponseCode>, ICreateCustomGameSessionRequest, IServiceRequest, IAnswerOnComplete<SessionCreationResponseCode>
	{
		protected override byte OperationCode => 143;

		public CreateCustomGameSessionRequest()
			: base("strCustomGameError", "strCustomGameSessionCreateFail", 0)
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

		protected override SessionCreationResponseCode ProcessResponse(OperationResponse response)
		{
			SessionCreationResponseCode sessionCreationResponseCode = (SessionCreationResponseCode)response.get_Item((byte)168);
			if (sessionCreationResponseCode == SessionCreationResponseCode.SessionCreated)
			{
				object obj = response.get_Item((byte)169);
			}
			return sessionCreationResponseCode;
		}

		void ICreateCustomGameSessionRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
