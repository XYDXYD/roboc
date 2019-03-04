using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonInviteEventListener : SocialEventListener<PlatoonInvite>, IPlatoonInviteEventListener, IServiceEventListener<PlatoonInvite>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonInviteReceived;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			string inviterName = (string)parameters[19];
			string displayName = (string)parameters[75];
			bool useCustomAvatar = (bool)parameters[13];
			int avatarId = (int)parameters[14];
			CacheDTO.platoonInvite = new PlatoonInvite(inviterName, displayName, new AvatarInfo(useCustomAvatar, avatarId));
			Invoke(CacheDTO.platoonInvite.Clone());
		}
	}
}
