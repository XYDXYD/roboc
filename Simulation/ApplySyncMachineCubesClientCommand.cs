using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class ApplySyncMachineCubesClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SyncMachineCubesDependency _dependency;

		[Inject]
		internal DestructionSyncReplayer _replayer
		{
			get;
			private set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (SyncMachineCubesDependency)dependency;
			return this;
		}

		public void Execute()
		{
			_replayer.Replay(_dependency);
		}
	}
}
