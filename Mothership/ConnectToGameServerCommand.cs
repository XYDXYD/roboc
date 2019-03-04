using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class ConnectToGameServerCommand : ICommand
	{
		private NetworkConfig _networkConfig;

		private int _hostPort;

		private string _hostIp;

		[Inject]
		internal NetworkEventRegistrationMothership networkEventRegistrationMothership
		{
			private get;
			set;
		}

		public void SetValues(string hostIp, int hostPort, NetworkConfig networkConfig)
		{
			_networkConfig = networkConfig;
			_hostIp = hostIp;
			_hostPort = hostPort;
		}

		public void SetEncryptionParams(byte[] encryptionParams)
		{
			networkEventRegistrationMothership.SetEncryptionParams(encryptionParams);
		}

		public void Execute()
		{
			networkEventRegistrationMothership.Start(_hostIp, _hostPort, _networkConfig);
		}
	}
}
