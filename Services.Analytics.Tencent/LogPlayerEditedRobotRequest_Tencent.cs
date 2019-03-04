using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerEditedRobotRequest_Tencent : WebServicesRequest, ILogPlayerEditedRobotRequest, IServiceRequest<EditedRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
		private EditedRobotDependency _dependency;

		protected override byte OperationCode => 212;

		public LogPlayerEditedRobotRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerEditedRobotError", 0)
		{
		}

		public void Inject(EditedRobotDependency dependency)
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
