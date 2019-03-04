using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class KickFromPlatoonRequest : SocialRequest, IKickFromPlatoonRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 14;

		public KickFromPlatoonRequest()
			: base("strRobocloudError", "strKickPlayerPlatError", 0)
		{
		}

		public void Inject(string userName)
		{
			_userName = userName;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			if (CacheDTO.platoon.isInPlatoon && CacheDTO.platoon.HasPlayer(_userName))
			{
				if (CacheDTO.platoon.Size <= 2)
				{
					CacheDTO.platoon = new Platoon();
				}
				else
				{
					CacheDTO.platoon.RemoveMember(_userName);
				}
				PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[1] = _userName;
			return val;
		}
	}
}
