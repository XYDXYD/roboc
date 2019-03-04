using Svelto.Command;
using Svelto.IoC;

namespace Mothership.GUI
{
	internal class ZeroHUDCPUGaugesCommand : ICommand
	{
		private bool _lockToZero;

		[Inject]
		internal HUDCPUPowerGaugePresenter cpuGauge
		{
			private get;
			set;
		}

		[Inject]
		internal HUDDamageBoostPresenter damageBoostPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal HUDHealthPresenter healthPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal HUDSpeedPreseneter speedPresenter
		{
			private get;
			set;
		}

		[Inject]
		private HUDMassPresenter massPresenter
		{
			get;
			set;
		}

		public ICommand Inject(bool lockToZero)
		{
			_lockToZero = lockToZero;
			return this;
		}

		public void Execute()
		{
			cpuGauge.ForceDisplayedCpuToZero(_lockToZero);
			damageBoostPresenter.ForceDisplayedDamageBoostToZero(_lockToZero);
			healthPresenter.ForceDisplayedHealthToZero(_lockToZero);
			speedPresenter.ForceDisplayedHealthToZero(_lockToZero);
			massPresenter.ForceDisplayToZero(_lockToZero);
		}
	}
}
