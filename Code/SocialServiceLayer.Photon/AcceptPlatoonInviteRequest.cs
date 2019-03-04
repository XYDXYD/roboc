using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class AcceptPlatoonInviteRequest : SocialRequest, IAcceptPlatoonInviteRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 11;

		public AcceptPlatoonInviteRequest()
			: base("strRobocloudError", "strAcceptPlatInviteError", 0)
		{
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			string parameter = parameters.GetParameter<string>(SocialParameterCode.PlatoonId);
			string parameter2 = parameters.GetParameter<string>(SocialParameterCode.PlatoonLeader);
			PlatoonMember[] parameter3 = parameters.GetParameter<PlatoonMember[]>(SocialParameterCode.UserList);
			CacheDTO.platoonInvite = null;
			CacheDTO.platoon = new Platoon(parameter, parameter2);
			for (int i = 0; i < parameter3.Length; i++)
			{
				CacheDTO.platoon.AddMember(parameter3[i]);
			}
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}
	}
}
