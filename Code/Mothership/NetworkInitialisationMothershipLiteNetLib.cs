using Network;
using RCNetwork.Events;
using RCNetwork.UNet.Client;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Utility;

namespace Mothership
{
	internal class NetworkInitialisationMothershipLiteNetLib : NetworkEventRegistrationMothership, IInitialize
	{
		private NetworkConnectionClientLiteNetLib _networkConnection;

		private byte[] _encryptionParams;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			RegisterRemoteEvents(networkEventManager, commandFactory);
		}

		public override void SetEncryptionParams(byte[] encryptionParams)
		{
			_encryptionParams = encryptionParams;
		}

		public override void Start(string hostIp, int hostPort, NetworkConfig networkConfig)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			EventDispatcher val = new EventDispatcher();
			if (NetworkClient.active)
			{
				Console.LogWarning("A network connection has been left open before going to battle - shutting down");
				NetworkClient.ShutdownAll();
			}
			RobocraftNetworkClient networkClient = new RobocraftNetworkClient();
			NetworkConnectionStatusHandlerClient networkConnectionStatusHandlerClient = new NetworkConnectionStatusHandlerClient(val);
			networkConnectionStatusHandlerClient.SetHostParameters(hostIp, hostPort);
			NetworkConnectionEventHandlerLiteNetLibClient networkConnectionEventHandlerLiteNetLibClient = new NetworkConnectionEventHandlerLiteNetLibClient(networkClient, networkConnectionStatusHandlerClient);
			networkConnectionEventHandlerLiteNetLibClient.SetMaxDelay(networkConfig.MaxDelay);
			_networkConnection = new NetworkConnectionClientLiteNetLib(networkClient);
			new EventRouterClientLiteNetLib(networkClient, networkEventManager as NetworkEventManagerClientLiteNetLib);
			RegisterEvents(commandFactory, val);
			_encryptionParams = new byte[2];
			_networkConnection.Connect(hostIp, hostPort, networkConfig, _encryptionParams);
		}

		public override void Stop()
		{
			_networkConnection.Disconnect();
		}
	}
}
