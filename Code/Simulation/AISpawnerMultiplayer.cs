using Battle;
using Svelto.Command;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class AISpawnerMultiplayer
	{
		[Inject]
		internal MachineSpawnDispatcher _machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal LocalAIsContainer _localAIs
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory _commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer _playerTeams
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader _machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers _battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer _playerNames
		{
			private get;
			set;
		}

		public void SetupAIEntities()
		{
			ICollection<PlayerDataDependency> expectedPlayers = _battlePlayers.GetExpectedPlayers();
			foreach (PlayerDataDependency item in expectedPlayers)
			{
				if (item.AiPlayer)
				{
					int playerId = _playerNames.GetPlayerId(item.PlayerName);
					if (_localAIs.IsAIHostedLocally(playerId))
					{
						PreloadedMachine preloadedMachine = _machinePreloader.GetPreloadedMachine(item.PlayerName);
						RegisterAIMachineCommand registerAIMachineCommand = _commandFactory.Build<RegisterAIMachineCommand>();
						registerAIMachineCommand.Initialise(playerId, item.TeamId, item.PlayerName, preloadedMachine, 25f, 90f, _playerTeams.IsMyTeam(item.TeamId), "Spawn", "Explosion");
						registerAIMachineCommand.Execute();
					}
				}
			}
		}
	}
}
