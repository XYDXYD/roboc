using Simulation.SinglePlayer.ServerMock;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class HealingAssistBonusServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private HealingAssistBonusRequestDependency _dependency;

		[Inject]
		public TeamDeathMatchAIScoreServerMock teamDeathMatchAIScoreServerMock
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as HealingAssistBonusRequestDependency);
			return this;
		}

		public void Execute()
		{
			teamDeathMatchAIScoreServerMock.UpdateStats(_dependency.healingPlayerId, InGameStatId.HealAssist, 1);
		}
	}
}
