using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadTutorialStatusRequest : WebServicesRequest<LoadTutorialStatusData>, ILoadTutorialStatusRequest, IServiceRequest, IAnswerOnComplete<LoadTutorialStatusData>
	{
		protected override byte OperationCode => 122;

		public LoadTutorialStatusRequest()
			: base("strGenericError", "strLoadTutorialStatusRequestFailed", 0)
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

		protected override LoadTutorialStatusData ProcessResponse(OperationResponse response)
		{
			bool inProgress_ = (bool)response.Parameters[140];
			bool completed_ = (bool)response.Parameters[141];
			bool skipped_ = (bool)response.Parameters[142];
			return new LoadTutorialStatusData(inProgress_, skipped_, completed_);
		}
	}
}
