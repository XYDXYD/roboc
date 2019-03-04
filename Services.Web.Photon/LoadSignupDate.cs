using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadSignupDate : WebServicesRequest<DateTime>, ILoadSignupDate, IServiceRequest, IAnswerOnComplete<DateTime>
	{
		protected override byte OperationCode => 63;

		public LoadSignupDate()
			: base("strRobocloudError", "strUnableGetSignupDate", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override DateTime ProcessResponse(OperationResponse response)
		{
			long fileTime = (long)response.Parameters[71];
			return DateTime.FromFileTimeUtc(fileTime);
		}
	}
}
