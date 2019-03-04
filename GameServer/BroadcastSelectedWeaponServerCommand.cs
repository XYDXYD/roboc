using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace GameServer
{
	internal sealed class BroadcastSelectedWeaponServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SelectWeaponDependency _dependency;

		[Inject]
		public INetworkEventManagerServer eventManagerServer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SelectWeaponDependency);
			return this;
		}

		public void Execute()
		{
			eventManagerServer.BroadcastEventToAllPlayersExcept(NetworkEvent.BroadcastWeaponSelect, _dependency.senderId, _dependency);
		}
	}
}
