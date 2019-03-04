using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal class SendMachineDestroyedClientCommand : IInjectableCommand<KillDependency>, ICommand
	{
		private KillDependency _dependency;

		[Inject]
		internal INetworkEventManagerClient networkEventManagerClient
		{
			private get;
			set;
		}

		public void Execute()
		{
			networkEventManagerClient.SendEventToServer(NetworkEvent.MachineDestroyed, _dependency);
		}

		public ICommand Inject(KillDependency dependency)
		{
			_dependency = dependency;
			return this;
		}
	}
}
