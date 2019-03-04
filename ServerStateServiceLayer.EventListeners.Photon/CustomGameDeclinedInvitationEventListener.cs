using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameDeclinedInvitationEventListener : ServerStateEventListener<DeclineInviteToSessionData>, ICustomGameDeclinedInvitationEventListener, IServerStateEventListener<DeclineInviteToSessionData>, IServiceEventListener<DeclineInviteToSessionData>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameDeclinedInvitation;

		protected override void ParseData(EventData eventData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[190];
			string playerWhoDeclined_ = (string)val.get_Item((object)"UserName");
			DeclineInviteToSessionData data = new DeclineInviteToSessionData(playerWhoDeclined_);
			Invoke(data);
		}
	}
}
