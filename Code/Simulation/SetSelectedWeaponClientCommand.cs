using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SetSelectedWeaponClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SelectWeaponDependency _dependency;

		[Inject]
		internal SwitchWeaponObserver switchDispatcher
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SelectWeaponDependency);
			return this;
		}

		public void Execute()
		{
			switchDispatcher.RemoteSwitchWeapon(_dependency.machineId, _dependency.category);
		}
	}
}
