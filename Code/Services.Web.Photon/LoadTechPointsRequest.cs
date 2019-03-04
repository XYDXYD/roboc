using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadTechPointsRequest : WebServicesCachedRequest<int>, ILoadTechPointsRequest, IServiceRequest, IAnswerOnComplete<int>
	{
		protected override byte OperationCode => 187;

		public LoadTechPointsRequest()
			: base("strRobocloudError", "strUnableLoadTechPoints", 0)
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

		protected override int ProcessResponse(OperationResponse response)
		{
			return Convert.ToInt32(response.Parameters[214]);
		}

		void ILoadTechPointsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
