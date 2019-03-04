using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadTutorialStageRequest : WebServicesRequest<LoadTutorialStageData>, ILoadTutorialStageRequest, IServiceRequest, IAnswerOnComplete<LoadTutorialStageData>
	{
		protected override byte OperationCode => 123;

		public LoadTutorialStageRequest()
			: base("strGenericError", "strLoadTutorialStageRequestFailed", 0)
		{
		}

		public override void Execute()
		{
			int? tutorialstage = CacheDTO.tutorialstage;
			if (tutorialstage.HasValue)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					LoadTutorialStageData obj = new LoadTutorialStageData(CacheDTO.tutorialstage.Value);
					base.answer.succeed(obj);
				}
			}
			else
			{
				base.Execute();
			}
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

		protected override LoadTutorialStageData ProcessResponse(OperationResponse response)
		{
			int stage_ = (int)response.Parameters[143];
			return new LoadTutorialStageData(stage_);
		}

		void ILoadTutorialStageRequest.ClearCache()
		{
			CacheDTO.tutorialstage = null;
		}
	}
}
