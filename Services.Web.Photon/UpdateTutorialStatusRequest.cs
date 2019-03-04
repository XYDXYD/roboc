using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class UpdateTutorialStatusRequest : WebServicesRequest, IUpdateTutorialStatusRequest, IServiceRequest<UpdateTutorialStatusData>, IAnswerOnComplete, IServiceRequest
	{
		private UpdateTutorialStatusData _dependency;

		protected override byte OperationCode => 121;

		public UpdateTutorialStatusRequest()
			: base("strRobocloudError", "strUpdateTutorialStatusRequestFailed", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(141, _dependency.completed);
			dictionary.Add(140, _dependency.inProgress);
			dictionary.Add(142, _dependency.skipped);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		void IServiceRequest<UpdateTutorialStatusData>.Inject(UpdateTutorialStatusData dependency)
		{
			_dependency = dependency;
		}
	}
}
