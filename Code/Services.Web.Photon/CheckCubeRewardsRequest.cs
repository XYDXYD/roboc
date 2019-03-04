using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class CheckCubeRewardsRequest : WebServicesRequest<string[]>, ICheckCubeRewardsRequest, IServiceRequest, IAnswerOnComplete<string[]>
	{
		protected override byte OperationCode => 206;

		public CheckCubeRewardsRequest()
			: base("strRobocloudError", "strCheckCubeRewardsError", 3)
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

		protected override string[] ProcessResponse(OperationResponse response)
		{
			return (string[])response.Parameters[216];
		}
	}
}
