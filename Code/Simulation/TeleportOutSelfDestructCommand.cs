using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class TeleportOutSelfDestructCommand : ICommand
	{
		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		public void Execute()
		{
			commandFactory.Build<OnSelfDestructClientCommand>().Execute();
			eventManagerClient.SendEventToServer(NetworkEvent.SelfDestructClassicMode, new NetworkDependency());
			commandFactory.Build<SwitchToMothershipCommand>().Execute();
		}
	}
}
