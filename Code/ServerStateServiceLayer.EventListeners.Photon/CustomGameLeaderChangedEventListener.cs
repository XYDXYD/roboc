using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameLeaderChangedEventListener : ServerStateEventListener<string>, ICustomGameLeaderChangedEventListener, IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameLeaderChanged;

		protected override void ParseData(EventData eventData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[182];
			string data = (string)val.get_Item((object)"NewLeader");
			Invoke(data);
		}
	}
}
