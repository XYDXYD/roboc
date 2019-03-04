using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonMemberAvatarChangedEventListener : PhotonEventListener<PlatoonMemberChangedAvatarUpdate>, IPlatoonMemberAvatarChangedEventListener, IServiceEventListener<PlatoonMemberChangedAvatarUpdate>, IServiceEventListenerBase
	{
		public override int EventCode => 14;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			string parameter = parameters.GetParameter<string>(SocialParameterCode.UserName);
			bool parameter2 = parameters.GetParameter<bool>(SocialParameterCode.UseCustomAvatar);
			int parameter3 = parameters.GetParameter<int>(SocialParameterCode.AvatarId);
			if (CacheDTO.platoon.HasPlayer(parameter))
			{
				PlatoonMember memberData = CacheDTO.platoon.GetMemberData(parameter);
				AvatarInfo avatarInfo2 = memberData.AvatarInfo = new AvatarInfo(parameter2, parameter3);
				Invoke(new PlatoonMemberChangedAvatarUpdate(parameter, avatarInfo2));
				PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			}
		}
	}
}
