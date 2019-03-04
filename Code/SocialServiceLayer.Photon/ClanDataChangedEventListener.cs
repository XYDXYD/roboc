using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ClanDataChangedEventListener : SocialEventListener<ClanInfo>, IClanDataChangedEventListener, IServiceEventListener<ClanInfo>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanDataChanged;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			if (parameters.ContainsKey(32))
			{
				CacheDTO.MyClanInfo.ClanDescription = (string)parameters[32];
			}
			if (parameters.ContainsKey(34))
			{
				CacheDTO.MyClanInfo.ClanType = (ClanType)parameters[34];
			}
			Invoke(CacheDTO.MyClanInfo);
		}
	}
}
