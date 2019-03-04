using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class ReplaceDailyQuestRequest : WebServicesRequest, IReplaceDailyQuestRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _questID = string.Empty;

		protected override byte OperationCode => 67;

		public ReplaceDailyQuestRequest()
			: base("strRobocloudError", "strErrorReplacingDailyQuestRequest", 0)
		{
		}

		public void Inject(string questID)
		{
			_questID = questID;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[77] = _questID;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
