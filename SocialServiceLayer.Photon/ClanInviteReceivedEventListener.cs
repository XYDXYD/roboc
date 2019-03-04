using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ClanInviteReceivedEventListener : SocialEventListener<ClanInvite, ClanInvite[]>, IClanInviteReceivedEventListener, IServiceEventListener<ClanInvite, ClanInvite[]>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanInviteReceived;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			string inviterName = (string)parameters[1];
			string inviterDisplayName = (string)parameters[75];
			string clanName = (string)parameters[31];
			int clanSize = (int)parameters[35];
			bool useCustomAvatar = (bool)parameters[13];
			int avatarId = (int)parameters[14];
			ClanInvite clanInvite = new ClanInvite(inviterName, inviterDisplayName, clanName, clanSize, new AvatarInfo(useCustomAvatar, avatarId));
			if (!CacheDTO.ClanInvites.ContainsKey(clanInvite.ClanName))
			{
				CacheDTO.ClanInvites.Add(clanInvite.ClanName, clanInvite);
			}
			Invoke(clanInvite, CacheDTO.ClanInvites.Values.ToArray());
		}
	}
}
