using Mothership;
using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.Context;
using Svelto.IoC;
using Utility;

namespace RCNetwork.UNet.Client
{
	internal sealed class NetworkInitialisationTestClientLiteNetLib : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, INetworkInitialisationTestClient
	{
		private INetworkConnectionClient _networkConnection;

		private byte[] _encryptionParams;

		[Inject]
		internal INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public void OnFrameworkInitialized()
		{
			Initialise();
		}

		public void OnFrameworkDestroyed()
		{
			if (_networkConnection != null)
			{
				Console.LogError("Test client still active on teardown - shutting down now");
				ShutDown();
			}
		}

		public void SetEncryptionParams(byte[] encryptionParams)
		{
			_encryptionParams = encryptionParams;
		}

		public void Start(string hostIP, int hostPort, NetworkConfig networkConfig)
		{
			AddUnityNetworkFrameworkComponents(networkConfig);
			_encryptionParams = new byte[2];
			_networkConnection.Connect(hostIP, hostPort, networkConfig, _encryptionParams);
		}

		public void Stop()
		{
			if (_networkConnection != null)
			{
				ShutDown();
			}
		}

		private void ShutDown()
		{
			_networkConnection.Disconnect();
			_networkConnection = null;
		}

		private void Initialise()
		{
			RegisterRemoteEvents();
		}

		private void AddUnityNetworkFrameworkComponents(NetworkConfig networkConfig)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			EventDispatcher val = new EventDispatcher();
			RobocraftNetworkClient networkClient = new RobocraftNetworkClient();
			NetworkConnectionStatusHandlerClient connectionStatusHandler = new NetworkConnectionStatusHandlerClient(val);
			NetworkConnectionEventHandlerLiteNetLibClient networkConnectionEventHandlerLiteNetLibClient = new NetworkConnectionEventHandlerLiteNetLibClient(networkClient, connectionStatusHandler);
			new EventRouterClientLiteNetLib(networkClient, networkEventManager as NetworkEventManagerClientLiteNetLib);
			_networkConnection = new NetworkConnectionClientLiteNetLib(networkClient);
			networkConnectionEventHandlerLiteNetLibClient.SetMaxDelay(networkConfig.MaxDelay);
			val.Add<NetworkEvent>(NetworkEvent.OnConnectedToServer, commandFactory.Build<ClientConnectionSucceededCommand>());
			val.Add<NetworkEvent>(NetworkEvent.OnFailedToConnectToServer, commandFactory.Build<ClientConnectionFailedCommand>());
			val.Add<NetworkEvent>(NetworkEvent.OnDisconnectedFromServer, commandFactory.Build<ClientConnectionFailedCommand>());
		}

		private void RegisterRemoteEvents()
		{
			networkEventManager.RegisterEvent<MachineDefinitionDependency>(NetworkEvent.TestConnection, commandFactory.Build<ReceiveTestDataClientCommand>());
		}
	}
}
