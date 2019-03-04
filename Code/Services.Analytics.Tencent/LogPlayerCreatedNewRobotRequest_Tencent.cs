using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerCreatedNewRobotRequest_Tencent : WebServicesRequest, ILogPlayerCreatedNewRobotRequest, IServiceRequest<CreatedNewRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
		private CreatedNewRobotDependency _dependency;

		protected override byte OperationCode => 211;

		public LogPlayerCreatedNewRobotRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerCreatedNewRobotError", 0)
		{
		}

		public void Inject(CreatedNewRobotDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[101] = _dependency.ToDictionary();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
