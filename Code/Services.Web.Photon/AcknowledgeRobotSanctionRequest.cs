using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class AcknowledgeRobotSanctionRequest : WebServicesRequest, IAcknowledgeRobotSanctionRequest, IServiceRequest<RobotSanctionData>, IServiceRequest
	{
		private RobotSanctionData _robotSanctionData;

		protected override byte OperationCode => 175;

		public AcknowledgeRobotSanctionRequest()
			: base("strRobocloudError", "strAcknowledgeRobotSanctionRequestError", 0)
		{
		}

		public void Inject(RobotSanctionData robotSanctionData)
		{
			_robotSanctionData = robotSanctionData;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[102] = _robotSanctionData.Serialise();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
