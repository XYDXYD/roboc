using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SendSelectedWeaponClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SelectWeaponDependency _dependency;

		[Inject]
		public INetworkEventManagerClient networkEventManager
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
			networkEventManager.SendEventToServer(NetworkEvent.WeaponSelect, _dependency);
		}
	}
}
