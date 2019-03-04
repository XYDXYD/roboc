using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class RemovedFromClanEventListener : SocialEventListener, IRemovedFromClanEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.RemovedFromClan;

		protected override void ParseData(EventData eventData)
		{
			CacheDTO.MyClanInfo = null;
			CacheDTO.MyClanMembers = new Dictionary<string, ClanMember>();
			Invoke();
		}
	}
}
