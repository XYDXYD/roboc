internal interface INetworkConnectionServer
{
	void StartServer(int maxConnections, int hostPort, bool useNat, NetworkConfig networkConfig, byte[] encryptionParams);

	void StopServer(bool willReconnectAfter = false);
}
