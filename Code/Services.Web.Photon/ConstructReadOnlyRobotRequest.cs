using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class ConstructReadOnlyRobotRequest : WebServicesRequest, IConstructReadOnlyRobotRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private int _robotID = -1;

		protected override byte OperationCode => 9;

		public ConstructReadOnlyRobotRequest()
			: base("strRobocloudError", "strUnableToConstructRobot", 0)
		{
		}

		public void Inject(int robotID)
		{
			_robotID = robotID;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[45] = _robotID;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
