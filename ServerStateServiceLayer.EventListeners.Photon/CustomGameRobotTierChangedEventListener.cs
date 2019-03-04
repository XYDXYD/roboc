using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameRobotTierChangedEventListener : ServerStateEventListener<CustomGameRobotTierChangedEventData>, ICustomGameRobotTierChangedEventListener, IServerStateEventListener<CustomGameRobotTierChangedEventData>, IServiceEventListener<CustomGameRobotTierChangedEventData>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameRobotTierChanged;

		protected override void ParseData(EventData eventData)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			Hashtable val = eventData.Parameters[67];
			string sessionID_ = (string)val.get_Item((object)"sessionInWhichRobotTierChange");
			string targetUserName_ = (string)val.get_Item((object)"target");
			int newTier_ = Convert.ToInt32(val.get_Item((object)"tier"));
			CustomGameRobotTierChangedEventData data = new CustomGameRobotTierChangedEventData(sessionID_, targetUserName_, newTier_);
			Invoke(data);
		}
	}
}
