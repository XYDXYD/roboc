using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameKickedFromSessionEventListener : ServerStateEventListener<KickedFromCustomGameSessionData>, ICustomGameKickedFromSessionEventListener, IServerStateEventListener<KickedFromCustomGameSessionData>, IServiceEventListener<KickedFromCustomGameSessionData>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameKickedFromSession;

		protected override void ParseData(EventData eventData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[184];
			string sessionID_ = (string)val.get_Item((object)"Session");
			bool wasinvited_ = (bool)val.get_Item((object)"WasInvited");
			KickedFromCustomGameSessionData data = new KickedFromCustomGameSessionData(sessionID_, wasinvited_);
			Invoke(data);
		}
	}
}
