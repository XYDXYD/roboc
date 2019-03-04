using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class MachineFunctionalImplementor : IMachineFunctionalComponent
	{
		private DispatchOnChange<bool> _onRectifyingStateChanged;

		public bool functionalsEnabled
		{
			get
			{
				return _onRectifyingStateChanged.get_value();
			}
			set
			{
				_onRectifyingStateChanged.set_value(value);
			}
		}

		public DispatchOnChange<bool> onFunctionalsEnabled => _onRectifyingStateChanged;

		public MachineFunctionalImplementor(int machineId)
		{
			_onRectifyingStateChanged = new DispatchOnChange<bool>(machineId);
			_onRectifyingStateChanged.set_value(true);
		}
	}
}
