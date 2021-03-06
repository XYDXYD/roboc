using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class MakePlayerInvisibleClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private CloakModuleEventDependency _dependency;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal HUDPlayerIDManager hudPlayerIdManager
		{
			private get;
			set;
		}

		[Inject]
		internal RemotePlayerBecomeInvisibleObservable RemotePlayerBecomeInvisibleObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as CloakModuleEventDependency);
			return this;
		}

		public void Execute()
		{
			int playerId = _dependency.playerId;
			if (!playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId))
			{
				hudPlayerIdManager.ActivatePlayerHUDWidget(playerId, activate: false);
			}
			RemotePlayerBecomeInvisibleObservable.Dispatch(ref playerId);
		}
	}
}
