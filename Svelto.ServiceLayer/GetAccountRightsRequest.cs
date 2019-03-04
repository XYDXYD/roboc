using ExitGames.Client.Photon;
using Services.Web.Photon;
using System.Collections.Generic;

namespace Svelto.ServiceLayer
{
	internal sealed class GetAccountRightsRequest : WebServicesCachedRequest<AccountRights>, IGetAccountRightsRequest, IServiceRequest, IAnswerOnComplete<AccountRights>
	{
		protected override byte OperationCode => 14;

		public GetAccountRightsRequest()
			: base("strRobocloudError", "strErrLoadingAccountData", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override AccountRights ProcessResponse(OperationResponse response)
		{
			bool moderator = (bool)response.Parameters[10];
			bool developer = (bool)response.Parameters[11];
			bool admin = (bool)response.Parameters[12];
			return new AccountRights(moderator, developer, admin);
		}
	}
}
