using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CheckIfCanJoinCustomGameQueueRequest : WebServicesCachedRequest<CheckIfCanJoinCustomGameQueueResponse>, ICheckIfCanJoinCustomGameQueueResponse, IServiceRequest, IAnswerOnComplete<CheckIfCanJoinCustomGameQueueResponse>
	{
		protected override byte OperationCode => 153;

		public CheckIfCanJoinCustomGameQueueRequest()
			: base("strCustomGameError", "strCustomGameCheckJoinQueueRequestError", 0)
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

		protected override CheckIfCanJoinCustomGameQueueResponse ProcessResponse(OperationResponse response)
		{
			return (CheckIfCanJoinCustomGameQueueResponse)response.get_Item((byte)168);
		}

		void ICheckIfCanJoinCustomGameQueueResponse.ClearCache()
		{
			ClearCache();
		}
	}
}
