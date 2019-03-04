using Network;

namespace Simulation
{
	internal sealed class NetworkMachineSyncLiteNetLibClient
	{
		private MachineSyncClient _machineSync;

		public NetworkMachineSyncLiteNetLibClient(NetworkClient networkClient, MachineSyncClient machineSyncClient)
		{
			networkClient.RegisterHandler(NetworkMsgType.RobotMotion, OnServerMessageReceived);
			_machineSync = machineSyncClient;
		}

		private void OnServerMessageReceived(global::Network.NetworkMessage netMsg)
		{
			NetworkMessage networkMessage = netMsg.ReadMessage<NetworkMessage>();
			_machineSync.ReceiveIndividualMachineDataFromServer(networkMessage.data);
		}
	}
}
