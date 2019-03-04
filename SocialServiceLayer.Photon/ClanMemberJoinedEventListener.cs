using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ClanMemberJoinedEventListener : SocialEventListener<ClanMember[], ClanMember>, IClanMemberJoinedEventListener, IServiceEventListener<ClanMember[], ClanMember>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanMemberJoined;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			string text = (string)parameters[1];
			string displayName = (string)parameters[75];
			bool flag = (bool)parameters[13];
			int avatarId = (!flag) ? ((int)parameters[14]) : 0;
			ClanMemberState clanMemberState = (ClanMemberState)parameters[37];
			int seasonXP = (int)parameters[48];
			if (CacheDTO.MyClanMembers.ContainsKey(text))
			{
				ClanMember clanMember = CacheDTO.MyClanMembers[text];
				clanMember.ClanMemberRank = ClanMemberRank.Member;
				clanMember.ClanMemberState = clanMemberState;
				clanMember.IsOnline = true;
			}
			else
			{
				CacheDTO.MyClanMembers.Add(text, new ClanMember(text, displayName, clanMemberState, new AvatarInfo(flag, avatarId), ClanMemberRank.Member, isOnline: true, seasonXP));
			}
			Invoke(CacheDTO.MyClanMembers.Values.ToArray(), CacheDTO.MyClanMembers[text]);
		}
	}
}
