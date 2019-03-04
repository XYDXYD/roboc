using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal class DisconnectedPlayerVoiceOver : IInitialize
	{
		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public GameStateClient gameStateClient
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerSpawnedOut += spawnDispatcher_OnPlayerDisconnected;
		}

		protected virtual int DisconnectedEvent()
		{
			return 171;
		}

		private void spawnDispatcher_OnPlayerDisconnected(SpawnOutParameters paramaters)
		{
			if (!gameStateClient.hasGameEnded && playerTeamsContainer.IsOnMyTeam(TargetType.Player, paramaters.playerId))
			{
				commandFactory.Build<PlayVOCommand>().Inject(AudioFabricEvent.StringEvents[DisconnectedEvent()]).Execute();
			}
		}
	}
}
