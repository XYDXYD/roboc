using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class ReceiveEndOfSyncClientCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		private IInitialSimulationGUIFlow _flow
		{
			get;
			set;
		}

		public void Execute()
		{
			_flow.OnReceiveEndOfSync();
		}
	}
}
