using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;

namespace Services.Web.Photon
{
	internal sealed class LoadTotalXPRequest : WebServicesRequest<uint[]>, ILoadTotalXPRequest, IServiceRequest, IAnswerOnComplete<uint[]>
	{
		protected override byte OperationCode => 83;

		public LoadTotalXPRequest()
			: base("strRobocloudError", "strUnableLoadTotalRPEarned", 3)
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

		protected override uint[] ProcessResponse(OperationResponse operationResponse)
		{
			uint num = Convert.ToUInt32(operationResponse.Parameters[8]);
			return new uint[1]
			{
				num
			};
		}
	}
}
