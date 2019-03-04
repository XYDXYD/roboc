using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CustomGameRobotTierChangedRequest : WebServicesCachedRequest, ICustomGameRobotTierChangedRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private int _tier;

		protected override byte OperationCode => 172;

		public CustomGameRobotTierChangedRequest()
			: base("strRobocloudError", "strCustomGameRobotTierChangeRequestErrorBody", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[67] = _tier;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}

		public void Inject(int dependency)
		{
			_tier = dependency;
		}
	}
}
