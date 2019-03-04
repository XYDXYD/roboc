using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ClanMemberDataChangedEventListener : SocialEventListener<ClanMember[], ClanMemberDataChangedEventContent>, IClanMemberDataChangedEventListener, IServiceEventListener<ClanMember[], ClanMemberDataChangedEventContent>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanMemberDataChanged;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			string text = (string)parameters[1];
			string displayName = (string)parameters[75];
			ClanMemberState? clanMemberState = (!parameters.ContainsKey(37)) ? null : ((ClanMemberState?)parameters[37]);
			ClanMemberRank? clanMemberRank = (!parameters.ContainsKey(38)) ? null : ((ClanMemberRank?)parameters[38]);
			bool? isOnline = (!parameters.ContainsKey(2)) ? null : ((bool?)parameters[2]);
			if (clanMemberRank.HasValue && clanMemberRank.Value == ClanMemberRank.Leader)
			{
				foreach (ClanMember value in CacheDTO.MyClanMembers.Values)
				{
					if (value.ClanMemberRank == ClanMemberRank.Leader)
					{
						value.ClanMemberRank = ClanMemberRank.Officer;
					}
				}
			}
			if (CacheDTO.MyClanMembers.ContainsKey(text))
			{
				ClanMember clanMember = CacheDTO.MyClanMembers[text];
				if (clanMemberState.HasValue)
				{
					clanMember.ClanMemberState = clanMemberState.Value;
				}
				if (clanMemberRank.HasValue)
				{
					clanMember.ClanMemberRank = clanMemberRank.Value;
				}
				if (isOnline.HasValue)
				{
					clanMember.IsOnline = isOnline.Value;
				}
				if (parameters.ContainsKey(13))
				{
					bool useCustomAvatar = (bool)parameters[13];
					int avatarId = 0;
					if (parameters.ContainsKey(14))
					{
						avatarId = (int)parameters[14];
					}
					AvatarInfo avatarInfo2 = clanMember.AvatarInfo = new AvatarInfo(useCustomAvatar, avatarId);
				}
			}
			ClanMemberDataChangedEventContent data = new ClanMemberDataChangedEventContent(text, displayName, isOnline, clanMemberRank);
			Invoke(CacheDTO.MyClanMembers.Values.ToArray(), data);
		}
	}
}
