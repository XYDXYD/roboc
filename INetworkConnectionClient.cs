internal interface INetworkConnectionClient
{
	void Connect(string hostIp, int hostPort, NetworkConfig networkConfig, byte[] encryptionParams);

	void Disconnect();
}
