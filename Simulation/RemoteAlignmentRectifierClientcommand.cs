using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class RemoteAlignmentRectifierClientcommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIdDependency _dependency;

		[Inject]
		public RemoteAlignmentRectifierManager remoteAlignmentRectifierManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as PlayerIdDependency);
			return this;
		}

		public void Execute()
		{
			remoteAlignmentRectifierManager.StartAlignmentEffect(_dependency.owner);
		}
	}
}
