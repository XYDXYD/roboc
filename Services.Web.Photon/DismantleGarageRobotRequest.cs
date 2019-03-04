using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class DismantleGarageRobotRequest : WebServicesRequest, IDismantleGarageRobotRequest, IServiceRequest<uint>, IAnswerOnComplete, IServiceRequest
	{
		private uint _deleteSlotId;

		protected override byte OperationCode => 42;

		public DismantleGarageRobotRequest()
			: base("strRobocloudError", "strErrorDismantleGarageRobot", 0)
		{
		}

		public void Inject(uint dependency)
		{
			_deleteSlotId = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[43] = Convert.ToInt32(_deleteSlotId);
			val.Parameters = dictionary;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse operationResponse)
		{
			int num = (int)operationResponse.Parameters[43];
		}
	}
}
