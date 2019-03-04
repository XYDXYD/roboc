namespace Mothership
{
	internal class TestConnectionParameters
	{
		public readonly string hostIP;

		public readonly int hostPort;

		public readonly NetworkConfig networkConfig;

		public readonly byte[] encryptionParams;

		public TestConnectionParameters(string _hostIP, int _hostPort, NetworkConfig _networkConfig, byte[] _encryptionParams)
		{
			hostIP = _hostIP;
			hostPort = _hostPort;
			networkConfig = _networkConfig;
			encryptionParams = _encryptionParams;
		}
	}
}
