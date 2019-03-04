using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class UpdateTutorialStageRequest : WebServicesRequest<bool>, IUpdateTutorialStageRequest, IServiceRequest<UpdateTutorialStageData>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private UpdateTutorialStageData _dependency;

		protected override byte OperationCode => 120;

		public UpdateTutorialStageRequest()
			: base("strRobocloudError", "strUpdateTutorialStageRequestFailed", 0)
		{
		}

		void IServiceRequest<UpdateTutorialStageData>.Inject(UpdateTutorialStageData dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(143, Convert.ToInt32(_dependency.stage));
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			CacheDTO.tutorialstage = _dependency.stage;
			return true;
		}
	}
}
