using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using System.Text;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class DevMessageEventListener : ServerStateEventListener<DevMessage>, IDevMessageEventListener, IServerStateEventListener<DevMessage>, IServiceEventListener<DevMessage>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.DevMessage;

		protected override void ParseData(EventData eventData)
		{
			byte[] bytes = (byte[])eventData.Parameters[2];
			DevMessage data = new DevMessage(Encoding.UTF8.GetString(bytes), Convert.ToInt32(eventData.Parameters[15]));
			Invoke(data);
		}
	}
}
