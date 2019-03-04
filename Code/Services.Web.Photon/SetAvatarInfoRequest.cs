using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class SetAvatarInfoRequest : WebServicesRequest, ISetAvatarInfoRequest, IServiceRequest<AvatarInfo>, IAnswerOnComplete, IServiceRequest
	{
		private AvatarInfo _avatarInfo;

		protected override byte OperationCode => 111;

		public SetAvatarInfoRequest()
			: base("strRobocloudError", "strErrorSavingAvatarInfo", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(129, _avatarInfo.AvatarId);
			dictionary.Add(130, _avatarInfo.UseCustomAvatar);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.Parameters = parameters;
			return val;
		}

		public void Inject(AvatarInfo avatarInfo)
		{
			_avatarInfo = avatarInfo;
		}

		protected override void SuccessfulResponse(OperationResponse response)
		{
			CacheDTO.localPlayerAvatarInfo = _avatarInfo;
			base.SuccessfulResponse(response);
		}
	}
}
