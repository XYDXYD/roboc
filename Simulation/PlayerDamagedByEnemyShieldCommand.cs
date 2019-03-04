using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class PlayerDamagedByEnemyShieldCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DamagedByEnemyShieldDependency _dependency;

		[Inject]
		public INetworkEventManagerClient networkManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as DamagedByEnemyShieldDependency);
			return this;
		}

		public void Execute()
		{
			networkManager.SendEventToServer(NetworkEvent.SendDamagedByEnemyShield, _dependency);
		}
	}
}
