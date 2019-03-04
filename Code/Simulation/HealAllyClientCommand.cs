using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class HealAllyClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private HealedAllyCubesDependency _dependency;

		[Inject]
		public INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		[Inject]
		public WeaponFireStateSync fireState
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as HealedAllyCubesDependency);
			return this;
		}

		public void Execute()
		{
			fireState.HealReportedToServer(_dependency, TargetType.Player);
			networkEventManager.SendEventToServer(NetworkEvent.HealAlly, _dependency);
		}
	}
}
