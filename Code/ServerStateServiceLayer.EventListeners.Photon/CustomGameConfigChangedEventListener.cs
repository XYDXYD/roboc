using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameConfigChangedEventListener : ServerStateEventListener<CustomGameConfigChangedData>, ICustomGameConfigChangedEventListener, IServerStateEventListener<CustomGameConfigChangedData>, IServiceEventListener<CustomGameConfigChangedData>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameConfigChanged;

		protected override void ParseData(EventData eventData)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[181];
			string fieldChanged_ = (string)val.get_Item((object)"Field");
			string newValue_ = (string)val.get_Item((object)"Value");
			CustomGameConfigChangedData data = new CustomGameConfigChangedData(fieldChanged_, newValue_);
			Invoke(data);
		}
	}
}
