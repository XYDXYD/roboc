using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class ClanInviteCancelledEventListener : SocialEventListener<ClanInvite[]>, IClanInviteCancelledEventListener, IServiceEventListener<ClanInvite[]>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanInviteCancelled;

		protected override void ParseData(EventData eventData)
		{
			string key = (string)eventData.Parameters[31];
			if (CacheDTO.ClanInvites.ContainsKey(key))
			{
				CacheDTO.ClanInvites.Remove(key);
			}
			ClanInvite[] array = new ClanInvite[CacheDTO.ClanInvites.Count];
			CacheDTO.ClanInvites.Values.CopyTo(array, 0);
			Invoke(array);
			Invoke(CacheDTO.ClanInvites.Values.ToArray());
		}
	}
}
