using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace GameServer
{
	internal sealed class BroadcastActivateModuleReadyEffectServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private ModuleReadyEffectDependency _dependency;

		[Inject]
		internal INetworkEventManagerServer networkManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as ModuleReadyEffectDependency);
			return this;
		}

		public void Execute()
		{
			networkManager.BroadcastEventToAllPlayersExcept(NetworkEvent.ActivateReadyEffect, _dependency.playerId, _dependency);
		}
	}
}
