using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerStartedGameRequest_Tencent : WebServicesRequest, ILogPlayerStartedGameRequest, IServiceRequest<LogPlayerStartedGameDependency>, IAnswerOnComplete, IServiceRequest
	{
		private LogPlayerStartedGameDependency _dependency;

		protected override byte OperationCode => 207;

		public LogPlayerStartedGameRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerStartGameError", 0)
		{
		}

		public void Inject(LogPlayerStartedGameDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[225] = _dependency.ToDictionary();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
