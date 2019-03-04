using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Hardware.Modules.PowerModule
{
	internal sealed class ActivatePowerModuleEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIdDependency _dependency;

		[Inject]
		internal PowerUpdateObservable observable
		{
			private get;
			set;
		}

		public void Execute()
		{
			int owner = _dependency.owner;
			observable.Dispatch(ref owner);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as PlayerIdDependency);
			return this;
		}
	}
}
