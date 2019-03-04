using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class SinglePlayerValidateWeaponFireServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DestroyCubeDependency _dependency;

		[Inject]
		public SinglePlayerWeaponFireValidator singlePlayerWeaponFireValidator
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as DestroyCubeDependency);
			return this;
		}

		public void Execute()
		{
			if (singlePlayerWeaponFireValidator.ValidateWeaponFire())
			{
				networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.DestroyCubesFull, _dependency);
			}
		}
	}
}
