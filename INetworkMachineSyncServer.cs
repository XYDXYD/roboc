internal interface INetworkMachineSyncServer
{
	void SendData(byte[] data, int excludedConnectionId);
}
