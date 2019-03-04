using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonInviteCancelledEventListener : SocialEventListener<string>, IPlatoonInviteCancelledEventListener, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonInviteCancelled;

		protected override void ParseData(EventData eventData)
		{
			string data = (string)eventData.Parameters[19];
			CacheDTO.platoonInvite = null;
			Invoke(data);
		}
	}
}
