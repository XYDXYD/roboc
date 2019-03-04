using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class StartConnectionTestEventListener : LobbyEventListener<Host, NetworkConfig>, IStartConnectionTestEventListener, IServiceEventListener<Host, NetworkConfig>, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.StartConnectionTest;

		protected override void ParseData(EventData eventData)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			Dictionary<byte, object> parameters = eventData.Parameters;
			string hostAddress_ = (string)parameters[6];
			int hostPort_ = (int)parameters[7];
			Host data = new Host(hostAddress_, hostPort_);
			Hashtable config = parameters[23];
			NetworkConfig data2 = new NetworkConfig(config);
			Invoke(data, data2);
		}
	}
}
