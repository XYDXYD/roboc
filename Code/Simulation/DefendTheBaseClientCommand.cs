using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class DefendTheBaseClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private ThreateningTheBaseCommandDependency _dependency;

		[Inject]
		internal DefendTheBaseBonusManager defenseBonusStatusManager
		{
			get;
			private set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as ThreateningTheBaseCommandDependency);
			return this;
		}

		public void Execute()
		{
			if (_dependency.isThreateningBase)
			{
				defenseBonusStatusManager.EnteredThreateningEnemyBaseArea(_dependency.playerId);
			}
			else
			{
				defenseBonusStatusManager.ExitedThreateningBaseArea(_dependency.playerId);
			}
		}
	}
}
