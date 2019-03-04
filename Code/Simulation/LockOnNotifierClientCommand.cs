using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class LockOnNotifierClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private LockOnNotifierDependency _dependency;

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as LockOnNotifierDependency);
			return this;
		}

		public void Execute()
		{
			eventManagerClient.SendEventToServer(NetworkEvent.LockOnNotification, _dependency);
		}
	}
}
