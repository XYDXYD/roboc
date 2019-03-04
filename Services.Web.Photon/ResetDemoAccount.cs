using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class ResetDemoAccount : WebServicesRequest, IResetDemoAccount, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 71;

		public ResetDemoAccount()
			: base("strRobocloudError", "strUnableRefreshDemoAcc", 2)
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

		protected override void ProcessResponse(OperationResponse response)
		{
			Console.Log("*** Recieved response for ResetDemoAccount ***");
		}
	}
}
