using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameInvitationEventListener : ServerStateEventListener<CustomGameInvitationData>, ICustomGameInvitationEventListener, IServerStateEventListener<CustomGameInvitationData>, IServiceEventListener<CustomGameInvitationData>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameInvitation;

		protected override void ParseData(EventData eventData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[172];
			string inviterName_ = (string)val.get_Item((object)"Inviter");
			string displayName_ = (string)val.get_Item((object)"DisplayName");
			string sessionID_ = (string)val.get_Item((object)"SessionID");
			bool useCustomAvatar = (bool)val.get_Item((object)"UseCustomAvatar");
			int avatarId = (int)val.get_Item((object)"AvatarID");
			bool invitedToTeamA_ = (bool)val.get_Item((object)"InvitedToTeamA");
			CustomGameInvitationData data = new CustomGameInvitationData(sessionID_, inviterName_, displayName_, useCustomAvatar, avatarId, invitedToTeamA_);
			Invoke(data);
		}
	}
}
