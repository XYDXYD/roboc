using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class RequestRespawnPointClientCommand : ICommand
	{
		[Inject]
		internal INetworkEventManagerClient networkEventManagerClient
		{
			private get;
			set;
		}

		public void Execute()
		{
			networkEventManagerClient.SendEventToServer(NetworkEvent.RequestRespawnPoint, new NetworkDependency());
		}
	}
}
