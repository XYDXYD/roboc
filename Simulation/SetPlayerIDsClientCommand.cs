using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal class SetPlayerIDsClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIDsAndNamesDependency _dependency;

		[Inject]
		private PlayerNamesContainer playerNames
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeams
		{
			get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (PlayerIDsAndNamesDependency)dependency;
			return this;
		}

		public void Execute()
		{
			foreach (KeyValuePair<int, PlayerNamesContainer.Player> item in _dependency.idToPlayer)
			{
				playerNames.RegisterPlayerName(item.Key, item.Value.name, item.Value.displayName);
			}
			playerNames.OnPlayerIDsReceived();
		}
	}
}
