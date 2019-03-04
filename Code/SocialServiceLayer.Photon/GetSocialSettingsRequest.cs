using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetSocialSettingsRequest : SocialRequest<Dictionary<string, object>>, IGetSocialSettingsRequest, IServiceRequest, IAnswerOnComplete<Dictionary<string, object>>
	{
		protected override byte OperationCode => 24;

		public GetSocialSettingsRequest()
			: base("strRobocloudError", "strGetSocialSetError", 0)
		{
		}

		public override void Execute()
		{
			if (CacheDTO.socialSettings != null)
			{
				base.answer.succeed(CacheDTO.socialSettings);
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
			return val;
		}

		protected override Dictionary<string, object> ProcessResponse(OperationResponse response)
		{
			return CacheDTO.socialSettings = (Dictionary<string, object>)response.Parameters[30];
		}
	}
}
