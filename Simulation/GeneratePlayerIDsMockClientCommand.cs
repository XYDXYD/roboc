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
	internal sealed class GeneratePlayerIDsMockClientCommand : ICommand
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
			TaskRunner.get_Instance().Run(AssignIDs());
		}

		private IEnumerator AssignIDs()
		{
			IRetrieveExpectedPlayersListRequest request = _lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>();
			TaskService<ReadOnlyDictionary<string, PlayerDataDependency>> task = new TaskService<ReadOnlyDictionary<string, PlayerDataDependency>>(request);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Cannot assign IDs to local machines, something is very wrong");
				yield break;
			}
			ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers = task.result;
			int nextId = 0;
			DictionaryEnumerator<string, PlayerDataDependency> enumerator = expectedPlayers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, PlayerDataDependency> current = enumerator.get_Current();
					string playerName = current.Value.PlayerName;
					string displayName = current.Value.DisplayName;
					_playerNamesContainer.RegisterPlayerName(nextId, playerName, displayName);
					nextId++;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			_playerNamesContainer.OnPlayerIDsReceived();
		}
	}
}
