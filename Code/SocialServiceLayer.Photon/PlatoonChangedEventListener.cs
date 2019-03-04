using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonChangedEventListener : SocialEventListener<Platoon>, IPlatoonChangedEventListener, IServiceEventListener<Platoon>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.NotUsed;

		protected override void ParseData(EventData eventData)
		{
			throw new NotImplementedException("This event listener is only ever raised internally");
		}
	}
}
