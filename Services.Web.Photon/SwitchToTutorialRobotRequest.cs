using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SwitchToTutorialRobotRequest : WebServicesRequest, ISwitchToTutorialRobotRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 124;

		public SwitchToTutorialRobotRequest()
			: base("strGenericError", "strTutorialSwitchToTutorialRobotRequestFailed", 0)
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
	}
}
