using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class HandleConnectedToServerSimulationCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal BattleLoadProgress BattleLoadProgress
		{
			private get;
			set;
		}

		public void Execute()
		{
			(BattleLoadProgress as BattleLoadProgressSimulation).Start();
		}
	}
}
