using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;

namespace Services.Web.Photon
{
	internal class LoadIfPlayerIsModeratorRequest : WebServicesRequest<string>, ILoadIfPlayerIsModeratorRequest, IServiceRequest, IAnswerOnComplete<string>
	{
		protected override byte OperationCode => 106;

		public LoadIfPlayerIsModeratorRequest()
			: base("strRobocloudError", "strUnableGetModPlayer", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override string ProcessResponse(OperationResponse response)
		{
			return Convert.ToString(response.Parameters[120]);
		}
	}
}
