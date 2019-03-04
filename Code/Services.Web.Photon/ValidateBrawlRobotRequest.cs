using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class ValidateBrawlRobotRequest : WebServicesCachedRequest<ValidateRobotForBrawlResult>, IValidateBrawlRobotRequest, IServiceRequest, IAnswerOnComplete<ValidateRobotForBrawlResult>
	{
		protected override byte OperationCode => 135;

		public ValidateBrawlRobotRequest()
			: base("strRobocloudError", "strErrorValidateBrawlRequestErrorTitle", 0)
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

		protected override ValidateRobotForBrawlResult ProcessResponse(OperationResponse response)
		{
			return (ValidateRobotForBrawlResult)response.get_Item((byte)154);
		}

		void IValidateBrawlRobotRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
