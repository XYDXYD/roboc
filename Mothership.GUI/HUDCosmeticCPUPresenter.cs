using Svelto.IoC;

namespace Mothership.GUI
{
	internal class HUDCosmeticCPUPresenter : IInitialize
	{
		private HUDCosmeticCPUView _view;

		private uint _localCurrentCCpu;

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		internal void SetView(HUDCosmeticCPUView view)
		{
			_view = view;
			_view.SetValue(0u, cpuPower.MaxCosmeticCpuPool);
		}

		public void OnDependenciesInjected()
		{
			RegisterEventListeners();
		}

		private void OnFrameworkDestroyed()
		{
			UnregisterEventListeners();
		}

		private void RegisterEventListeners()
		{
			cpuPower.RegisterOnCosmeticCPULoadChanged(OnCurrentCCpuLoadChange);
		}

		private void UnregisterEventListeners()
		{
			cpuPower.UnregisterOnCosmeticCPULoadChanged(OnCurrentCCpuLoadChange);
		}

		public void ForceDisplayToZero(bool lockToZero)
		{
			if (lockToZero)
			{
				UnregisterEventListeners();
				_view.SetValue(0u, cpuPower.MaxCosmeticCpuPool);
			}
			else
			{
				RegisterEventListeners();
			}
		}

		private void OnCurrentCCpuLoadChange(uint _currentCCpu)
		{
			_view.SetValue(_currentCCpu, cpuPower.MaxCosmeticCpuPool);
		}
	}
}
