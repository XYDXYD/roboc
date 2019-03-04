using Events.Dependencies;
using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Commands.Server
{
	internal sealed class BroadcastMultipleMissesServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private MultipleFireMissesDependency _dependency;

		[Inject]
		public INetworkEventManagerServer eventManagerServer
		{
			private get;
			set;
		}

		[Inject]
		public WeaponFireValidator weaponFireValidator
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as MultipleFireMissesDependency);
			return this;
		}

		public void Execute()
		{
			eventManagerServer.BroadcastEventToAllPlayersExceptUnreliable(NetworkEvent.MultipleFireMisses, _dependency.senderId, _dependency);
		}
	}
}
