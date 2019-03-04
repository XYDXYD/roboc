using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class OnPlayerReadyToRespawnMockCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal SinglePlayerRespawner respawner
		{
			private get;
			set;
		}

		public void Execute()
		{
			respawner.GetRespawnPoint();
		}
	}
}
