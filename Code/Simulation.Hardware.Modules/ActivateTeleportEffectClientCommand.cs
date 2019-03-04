using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Hardware.Modules
{
	internal sealed class ActivateTeleportEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private TeleportActivateEffectDependency _dependency;

		private NetworkPlayerBlinkedData _data = new NetworkPlayerBlinkedData(teleportStarted_: false, 0);

		[Inject]
		internal NetworkPlayerBlinkedObservable playerBlinkedObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as TeleportActivateEffectDependency);
			return this;
		}

		public void Execute()
		{
			_data.SetValues(_dependency.activate, _dependency.playerId);
			playerBlinkedObservable.Dispatch(ref _data);
		}
	}
}
