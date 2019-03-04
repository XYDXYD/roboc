using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ServerStateServiceLayer.Requests.Photon
{
	internal sealed class GetMaintenanceModeRequest : WebServicesRequest<MaintenanceModeData>, IGetMaintenanceModeRequest, IServiceRequest, IAnswerOnComplete<MaintenanceModeData>
	{
		protected override byte OperationCode => 20;

		public GetMaintenanceModeRequest()
			: base("strRobocloudError", "strMaintenanceMode", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override MaintenanceModeData ProcessResponse(OperationResponse response)
		{
			bool isInMaintenanceLcl = (bool)response.Parameters[20];
			string messageLcl = (string)response.Parameters[19];
			return new MaintenanceModeData(isInMaintenanceLcl, messageLcl);
		}
	}
}
