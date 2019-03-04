using RCNetwork.Events;
using RCNetwork.Server;
using Simulation.Hardware.Weapons;
using Simulation.SinglePlayer.ServerMock;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class AssistBonusServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private AssistBonusRequestDependency _dependency;

		[Inject]
		public TeamDeathMatchAIScoreServerMock teamDeathMatchAIScoreServerMock
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerServer networkEventManager
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (AssistBonusRequestDependency)dependency;
			return this;
		}

		public void Execute()
		{
			for (int i = 0; i < _dependency.awardedPlayerIds.Count; i++)
			{
				int num = _dependency.awardedPlayerIds[i];
				teamDeathMatchAIScoreServerMock.UpdateStats(num, InGameStatId.KillAssist, 1);
				if (playerTeams.IsMe(TargetType.Player, num))
				{
					networkEventManager.SendEventToPlayer(NetworkEvent.ConfirmedAssist, num, new KillDependency(_dependency.requesterPlayerId, num));
				}
			}
		}
	}
}
