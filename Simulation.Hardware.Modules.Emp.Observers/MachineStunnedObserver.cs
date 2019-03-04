using Svelto.Observer.IntraNamespace;

namespace Simulation.Hardware.Modules.Emp.Observers
{
	internal sealed class MachineStunnedObserver : Observer<MachineStunnedData>
	{
		public MachineStunnedObserver(MachineStunnedObservable observable)
			: base(observable)
		{
		}
	}
}
