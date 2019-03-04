using Svelto.Command;
using Svelto.Command.Dispatcher;

namespace Mothership.TechTree
{
	internal class LockingInputCommand : IDispatchableCommand, ICommand
	{
		private readonly bool _locked;

		private readonly ITechTreeViewDispatcherComponent _dispatcherComponent;

		public LockingInputCommand(bool locked, ITechTreeViewDispatcherComponent dispatcherComponent)
		{
			_locked = locked;
			_dispatcherComponent = dispatcherComponent;
		}

		public void Execute()
		{
			_dispatcherComponent.InputLocked.set_value(_locked);
		}
	}
}
