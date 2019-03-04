using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerClickedInRobotShopRequest_Tencent : WebServicesRequest, ILogPlayerClickedInRobotShopRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private int _clickCount = -1;

		protected override byte OperationCode => 214;

		public LogPlayerClickedInRobotShopRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerClickedRoboShopError", 0)
		{
		}

		public void Inject(int clickCount)
		{
			_clickCount = clickCount;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[226] = _clickCount;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
