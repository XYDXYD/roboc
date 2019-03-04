using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class UpdateShopRobotOffsetRequest : WebServicesRequest, IUpdateShotRobotOffsetRequest, IServiceRequest<UpdateShopRobotOffsetDependency>, IAnswerOnComplete, IServiceRequest
	{
		private UpdateShopRobotOffsetDependency _parameters;

		protected override byte OperationCode => 97;

		public UpdateShopRobotOffsetRequest()
			: base("strRobocloudError", "strRobotShopUpdateServiceError", 0)
		{
		}

		public void Inject(UpdateShopRobotOffsetDependency parameters)
		{
			_parameters = new UpdateShopRobotOffsetDependency(parameters);
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[45] = (int)_parameters.robotId;
			val.Parameters[104] = _parameters.cubesOffsetX;
			val.Parameters[105] = _parameters.cubesOffsetZ;
			val.Parameters[106] = _parameters.expectedLocationFirstCubeX;
			val.Parameters[107] = _parameters.expectedLocationFirstCubeY;
			val.Parameters[108] = _parameters.expectedLocationFirstCubeZ;
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
