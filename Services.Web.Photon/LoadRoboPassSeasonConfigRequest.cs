using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadRoboPassSeasonConfigRequest : WebServicesCachedRequest<RoboPassSeasonData>, ILoadRoboPassSeasonConfigRequest, IServiceRequest, IAnswerOnComplete<RoboPassSeasonData>
	{
		protected override byte OperationCode => 108;

		public LoadRoboPassSeasonConfigRequest()
			: base("strGenericError", "strLoadRoboPassSeasonConfigRequestError", 0)
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

		protected override RoboPassSeasonData ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			object obj = parameters[1];
			if (obj == null)
			{
				return null;
			}
			Dictionary<string, object> data = (Dictionary<string, object>)obj;
			return new RoboPassSeasonData(data);
		}

		void ILoadRoboPassSeasonConfigRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
