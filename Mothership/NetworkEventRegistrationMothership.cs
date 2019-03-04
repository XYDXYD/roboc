using RCNetwork.Events;
using RCNetwork.UNet.Client;
using Svelto.Command;
using Svelto.Command.Dispatcher;

namespace Mothership
{
	internal abstract class NetworkEventRegistrationMothership : INetworkInitialisationTestClient
	{
		protected void RegisterEvents(ICommandFactory commandFactory, IEventDispatcher eventDispatcher)
		{
			RegisterLocalEvents(commandFactory, eventDispatcher);
		}

		private void RegisterLocalEvents(ICommandFactory commandFactory, IEventDispatcher eventDispatcher)
		{
			eventDispatcher.Add<NetworkEvent>(NetworkEvent.OnFailedToConnectToServer, commandFactory.Build<FailedToConnectToGameServerInMothershipCommand>());
			eventDispatcher.Add<NetworkEvent>(NetworkEvent.OnDisconnectedFromServer, commandFactory.Build<DisconnectedFromGameServerInMothershipCommand>());
		}

		protected void RegisterRemoteEvents(INetworkEventManagerClient networkEventManager, ICommandFactory commandFactory)
		{
			networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.GameGuidValidated, commandFactory.Build<GameGuidValidatedCommand>());
			networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnConnectedToGameServer, commandFactory.Build<ValidateGameGuidClientCommand>());
			networkEventManager.RegisterEvent<StringCodeDependency>(NetworkEvent.WarnPlayer, commandFactory.Build<GameServerValidationFailedClientCommand>());
			networkEventManager.RegisterEvent<LoadingProgressDependency>(NetworkEvent.BroadcastLoadingProgress, commandFactory.Build<ReceiveLoadingProgressClientCommand>());
		}

		public abstract void SetEncryptionParams(byte[] encryptionParams);

		public abstract void Start(string hostIp, int hostPort, NetworkConfig networkConfig);

		public abstract void Stop();
	}
}
