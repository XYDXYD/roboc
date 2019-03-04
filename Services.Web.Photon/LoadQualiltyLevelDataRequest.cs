using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadQualiltyLevelDataRequest : WebServicesRequest<QualityLevelDataAnswerData>, ILoadQualiltyLevelDataRequest, IServiceRequest, IAnswerOnComplete<QualityLevelDataAnswerData>
	{
		protected override byte OperationCode => 104;

		public LoadQualiltyLevelDataRequest()
			: base("strRobocloudError", "strUnableLoadQualityLevelData", 0)
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

		protected override QualityLevelDataAnswerData ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> data = (Dictionary<string, Hashtable>)response.Parameters[1];
			return new QualityLevelDataAnswerData(data);
		}
	}
}
