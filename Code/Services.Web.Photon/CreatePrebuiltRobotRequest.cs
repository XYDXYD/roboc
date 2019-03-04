using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CreatePrebuiltRobotRequest : WebServicesRequest, ICreatePrebuiltRobotRequest, IServiceRequest<CreatePrebuiltRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
		private CreatePrebuiltRobotDependency _dependency;

		protected override byte OperationCode => 10;

		public CreatePrebuiltRobotRequest()
			: base("strRobocloudError", "strUnableToCreatePrebuiltRobot", 0)
		{
		}

		public void Inject(CreatePrebuiltRobotDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[46] = _dependency.robotData;
			val.Parameters[33] = _dependency.robotColourData;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
