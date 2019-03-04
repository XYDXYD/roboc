using Authentication;
using Battle;
using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;

internal class ValidateGameGuidClientCommand : IDispatchableCommand, ICommand
{
	[Inject]
	internal INetworkEventManagerClient networkEventManagerClient
	{
		private get;
		set;
	}

	[Inject]
	internal BattleParameters battleParameters
	{
		private get;
		set;
	}

	public void Execute()
	{
		networkEventManagerClient.SendEventToServer(NetworkEvent.ValidateGameGuid, new GameGuidDependency(battleParameters.BattleId, User.Username, (uint)(DateTime.UtcNow - battleParameters.ReceivedEnterBattleTime).TotalSeconds));
	}
}
