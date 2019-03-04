using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace GameServer
{
	internal sealed class SendTestDataServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private MachineDefinitionDependency _dependency;

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			private get;
			set;
		}

		public void Execute()
		{
			networkEventManagerServer.SendEventToPlayer(NetworkEvent.TestConnection, _dependency.senderId, _dependency);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as MachineDefinitionDependency);
			return this;
		}
	}
}
