using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class ClanMemberLeftEventListener : SocialEventListener<ClanMember[], ClanMember>, IClanMemberLeftEventListener, IServiceEventListener<ClanMember[], ClanMember>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanMemberLeft;

		protected override void ParseData(EventData eventData)
		{
			string key = (string)eventData.Parameters[1];
			if (CacheDTO.MyClanMembers != null)
			{
				ClanMember data = null;
				if (eventData.Parameters.ContainsKey(45))
				{
					string key2 = (string)eventData.Parameters[45];
					CacheDTO.MyClanMembers[key2].ClanMemberRank = ClanMemberRank.Leader;
				}
				if (CacheDTO.MyClanMembers.ContainsKey(key))
				{
					data = CacheDTO.MyClanMembers[key];
					CacheDTO.MyClanMembers.Remove(key);
				}
				Invoke(CacheDTO.MyClanMembers.Values.ToArray(), data);
			}
		}
	}
}
