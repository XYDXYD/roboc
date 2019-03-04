using Simulation.Hardware.Modules.Emp.Observers;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.Observer;
using System;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleInputDisablingComponent : IInputComponent, IWaitForFrameworkDestruction, IComponent
	{
		private MachineStunnedObserver _observer;

		public event Action<bool> machineStunned = delegate
		{
		};

		public unsafe EmpModuleInputDisablingComponent(MachineStunnedObserver machineStunnedObserver)
		{
			_observer = machineStunnedObserver;
			_observer.AddAction(new ObserverAction<MachineStunnedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_observer.RemoveAction(new ObserverAction<MachineStunnedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleMachineStunned(ref MachineStunnedData data)
		{
			this.machineStunned(data.isStunned);
		}
	}
}
