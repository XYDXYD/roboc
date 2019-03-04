using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SwitchToPreviousRobotIfNecessaryRequest : WebServicesRequest, ISwitchToPreviousRobotIfNecessaryRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 125;

		public SwitchToPreviousRobotIfNecessaryRequest()
			: base("strGenericError", "strSwitchToPreviousRobotIfNecessaryRequestFailed", 0)
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

		protected override void OnFailed(Exception e)
		{
			base.OnFailed(e);
		}
	}
}
