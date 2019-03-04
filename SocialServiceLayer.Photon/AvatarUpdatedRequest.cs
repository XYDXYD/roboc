using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class AvatarUpdatedRequest : SocialRequest, IAvatarUpdatedRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 6;

		public AvatarUpdatedRequest()
			: base("strRobocloudError", "strErrorSavingAvatarInfo", 0)
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
	}
}
