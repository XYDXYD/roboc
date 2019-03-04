using Network;

namespace Simulation
{
	internal sealed class DataSenderClientLiteNetLib : IDataSenderClientNetworkSpecific
	{
		private NetworkMessage _msg = new NetworkMessage();

		private NetworkClient _networkClient;

		public DataSenderClientLiteNetLib(NetworkClient networkClient)
		{
			_networkClient = networkClient;
		}

		public void SendDataToServer(MachineMotionDependency depenency)
		{
			if (_networkClient.isConnected)
			{
				_msg.type = 0;
				_msg.data = depenency.Serialise();
				_networkClient.SendByChannel(NetworkMsgType.RobotMotion, _msg, 2);
			}
		}
	}
}
