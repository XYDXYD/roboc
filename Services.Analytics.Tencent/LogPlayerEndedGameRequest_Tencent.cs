using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerEndedGameRequest_Tencent : WebServicesRequest, ILogPlayerEndedGameRequest, IServiceRequest<LogPlayerEndedGameDependency>, IAnswerOnComplete, IServiceRequest
	{
		private LogPlayerEndedGameDependency _dependency;

		protected override byte OperationCode => 208;

		public LogPlayerEndedGameRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerEndGameError", 0)
		{
		}

		public void Inject(LogPlayerEndedGameDependency dependency)
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
