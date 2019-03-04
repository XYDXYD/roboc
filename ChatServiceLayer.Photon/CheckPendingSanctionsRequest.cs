using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class CheckPendingSanctionsRequest : ChatRequest<bool>, ICheckPendingSanctionRequest, IServiceRequest, IAnswerOnComplete<bool>
	{
		protected override byte OperationCode => 15;

		public CheckPendingSanctionsRequest()
			: base("strRobocloudError", "strErrorCheckPendingSanction", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			return (bool)response.Parameters[28];
		}
	}
}
