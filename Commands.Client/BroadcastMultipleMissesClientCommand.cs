using Events.Dependencies;
using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Commands.Client
{
	internal sealed class BroadcastMultipleMissesClientCommand : IInjectableCommand<MultipleFireMissesDependency>, ICommand
	{
		private MultipleFireMissesDependency _dependency;

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		public ICommand Inject(MultipleFireMissesDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			eventManagerClient.SendEventToServerUnreliable(NetworkEvent.MultipleFireMisses, _dependency);
		}
	}
}
