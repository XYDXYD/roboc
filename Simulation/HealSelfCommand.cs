using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class HealSelfCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private HealedCubesDependency _dependency;

		[Inject]
		public INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as HealedCubesDependency);
			return this;
		}

		public void Execute()
		{
			networkEventManager.SendEventToServer(NetworkEvent.HealSelf, _dependency);
		}
	}
}
