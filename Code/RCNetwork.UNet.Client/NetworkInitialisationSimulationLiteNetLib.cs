using Battle;
using Network;
using RCNetwork.Events;
using Simulation;
using Simulation.Network;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Utility;

namespace RCNetwork.UNet.Client
{
	internal sealed class NetworkInitialisationSimulationLiteNetLib : NetworkEventRegistrationSimulation, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization
	{
		private IEntitySystemContext _root;

		private NetworkConnectionClientLiteNetLib _networkConnection;

		private MachineMotionSenderEngine _machineMotionSenderEngine;

		private InputSenderClient _inputSenderClient;

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

		[Inject]
		internal BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSyncClient machineSyncClient
		{
			private get;
			set;
		}

		public NetworkInitialisationSimulationLiteNetLib(IEntitySystemContext root, MachineMotionSenderEngine machineMotionSender)
		{
			_root = root;
			_machineMotionSenderEngine = machineMotionSender;
		}

		public void OnFrameworkInitialized()
		{
			InitialiseFramework();
		}

		public void OnFrameworkDestroyed()
		{
			if (_networkConnection != null)
			{
				_networkConnection.Disconnect();
				_networkConnection = null;
			}
			_inputSenderClient.Dispose();
		}

		private void BuildGameClasses()
		{
			_inputSenderClient = new InputSenderClient();
			_root.AddComponent(_inputSenderClient);
		}

		private void InitialiseFramework()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			EventDispatcher val = new EventDispatcher();
			NetworkClient networkClient = NetworkClient.allClients[0];
			if (!networkClient.isConnected)
			{
				Console.LogError("Game server connection lost between scenes");
				commandFactory.Build<DisplayServerDisconnectionErrorCommand>().Execute();
				return;
			}
			NetworkConnectionStatusHandlerClient networkConnectionStatusHandlerClient = new NetworkConnectionStatusHandlerClient(val);
			networkConnectionStatusHandlerClient.SetHostParameters(battleParameters.HostIP, battleParameters.HostPort);
			new NetworkConnectionEventHandlerLiteNetLibClientAlreadyConnected(networkClient, networkConnectionStatusHandlerClient);
			_networkConnection = new NetworkConnectionClientLiteNetLib(networkClient);
			new EventRouterClientLiteNetLib(networkClient, networkEventManager as NetworkEventManagerClientLiteNetLib);
			_machineMotionSenderEngine.dataSender = new DataSenderClientLiteNetLib(networkClient);
			new NetworkMachineSyncLiteNetLibClient(networkClient, machineSyncClient);
			BuildGameClasses();
			RegisterEvents(commandFactory, val, networkEventManager);
			val.Dispatch<NetworkEvent>(NetworkEvent.OnConnectedToServer);
		}
	}
}
