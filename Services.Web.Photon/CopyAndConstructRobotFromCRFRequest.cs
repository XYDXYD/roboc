using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CopyAndConstructRobotFromCRFRequest : WebServicesRequest, ICopyAndConstructRobotFromCRFRequest, IServiceRequest<CopyAndConstructRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
		private CopyAndConstructRobotDependency _dependency;

		protected override byte OperationCode => 166;

		public CopyAndConstructRobotFromCRFRequest()
			: base("strRobocloudError", "strUnableToCopyAndConstructRobotFromShop", 0)
		{
		}

		public void Inject(CopyAndConstructRobotDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = _dependency.crfID;
			val.Parameters[5] = (int)_dependency.robitsCost;
			val.Parameters[6] = (int)_dependency.ccCost;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
