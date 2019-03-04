using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Mothership
{
	internal class GarageOptionsPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private GarageOptionsView _view;

		[Inject]
		internal GarageChangedObserver garageObserver
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			garageObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			cpuPower.RegisterOnCPULoadChanged(ToggleCopyButton);
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			garageObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			cpuPower.UnregisterOnCPULoadChanged(ToggleCopyButton);
		}

		public void SetView(GarageOptionsView view)
		{
			_view = view;
		}

		private void DisplayGarageButtons(ref GarageSlotDependency currentGarageSlot)
		{
			_view.DisplayGarageButtons();
			ToggleCopyButton(currentGarageSlot.totalRobotCPU);
		}

		private void ToggleCopyButton(uint cpu)
		{
			_view.EnableCopyButton(cpu != 0);
		}
	}
}
