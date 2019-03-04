using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class SinglePlayerValidateWeaponFireEffectOnlyServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DestroyCubeEffectOnlyDependency _dependency;

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
			_dependency = (dependency as DestroyCubeEffectOnlyDependency);
			return this;
		}

		public void Execute()
		{
			if (singlePlayerWeaponFireValidator.ValidateWeaponFire())
			{
				networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.DestroyCubeEffectOnly, _dependency);
			}
		}
	}
}
