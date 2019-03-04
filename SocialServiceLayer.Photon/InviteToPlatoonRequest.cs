using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class InviteToPlatoonRequest : SocialRequest, IInviteToPlatoonRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _invitee;

		protected override byte OperationCode => 10;

		public InviteToPlatoonRequest()
			: base("strRobocloudError", "strSendPlatInviteError", 0)
		{
		}

		public void Inject(string invitee)
		{
			_invitee = invitee;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[1] = _invitee;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			string parameter = parameters.GetParameter<string>(SocialParameterCode.PlatoonId);
			PlatoonMember parameter2 = parameters.GetParameter<PlatoonMember>(SocialParameterCode.PlatoonMember);
			int parameter3 = parameters.GetParameter<int>(SocialParameterCode.AvatarId);
			bool parameter4 = parameters.GetParameter<bool>(SocialParameterCode.UseCustomAvatar);
			if (CacheDTO.platoon == null || !CacheDTO.platoon.isInPlatoon)
			{
				string username = User.Username;
				string displayName = User.DisplayName;
				CacheDTO.platoon = new Platoon(parameter, username);
				CacheDTO.platoon.AddMember(new PlatoonMember(username, displayName, PlatoonMember.MemberStatus.Ready, new AvatarInfo(useCustomAvatar: false, 0), 0L));
			}
			parameter2.AvatarInfo = new AvatarInfo(parameter4, parameter3);
			CacheDTO.platoon.AddMember(parameter2);
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
