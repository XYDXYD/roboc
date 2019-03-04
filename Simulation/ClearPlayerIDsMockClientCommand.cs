using LobbyServiceLayer;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal sealed class ClearPlayerIDsMockClientCommand : ICommand
	{
		[Inject]
		public ILobbyRequestFactory _lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public PlayerNamesContainer _playerNamesContainer
		{
			private get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run(ClearIDs());
		}

		private IEnumerator ClearIDs()
		{
			IRetrieveExpectedPlayersListRequest request = _lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>();
			TaskService<ReadOnlyDictionary<string, PlayerDataDependency>> task = new TaskService<ReadOnlyDictionary<string, PlayerDataDependency>>(request);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Cannot remove IDs to local machines");
				yield break;
			}
			ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers = task.result;
			DictionaryEnumerator<string, PlayerDataDependency> enumerator = expectedPlayers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, PlayerDataDependency> current = enumerator.get_Current();
					int playerId = _playerNamesContainer.GetPlayerId(current.Key);
					_playerNamesContainer.UnregisterPlayerName(playerId);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
