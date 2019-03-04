namespace RCNetwork.UNet.Client
{
	internal interface INetworkInitialisationTestClient
	{
		void SetEncryptionParams(byte[] encryptionParams);

		void Start(string hostIP, int hostPort, NetworkConfig networkConfig);

		void Stop();
	}
}
