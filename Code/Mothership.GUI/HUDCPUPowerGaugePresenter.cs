using DG.Tweening;
using DG.Tweening.Core;
using Svelto.Context;
using Svelto.IoC;
using System;

namespace Mothership.GUI
{
	internal sealed class HUDCPUPowerGaugePresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private HUDCPUPowerGauge _view;

		private uint _localCurrentCpu;

		private float _visibleCurrentCpu;

		private Sequence _mainSequence;

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_mainSequence = DOTween.Sequence();
			RegisterEventListners();
		}

		public void Initialise()
		{
			OnCurrentCpuChange(0u);
		}

		private void RegisterEventListners()
		{
			cpuPower.RegisterOnCPULoadChanged(OnCurrentCpuChange);
		}

		public void OnFrameworkDestroyed()
		{
			UnregisterEventListners();
		}

		private void UnregisterEventListners()
		{
			cpuPower.UnregisterOnCPULoadChanged(OnCurrentCpuChange);
		}

		internal void SetView(HUDCPUPowerGauge hUDCPUPowerGauge)
		{
			_view = hUDCPUPowerGauge;
		}

		internal void ForceDisplayedCpuToZero(bool enabled)
		{
			if (enabled)
			{
				UnregisterEventListners();
				OnCurrentCpuChange(0u);
			}
			else
			{
				RegisterEventListners();
			}
		}

		private unsafe void OnCurrentCpuChange(uint currentCpu)
		{
			_localCurrentCpu = currentCpu;
			TweenExtensions.Kill(_mainSequence, false);
			_mainSequence = DOTween.Sequence();
			TweenSettingsExtensions.SetRecyclable<Sequence>(_mainSequence, false);
			TweenSettingsExtensions.Append(_mainSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), (float)(double)_localCurrentCpu, _view.tweenDuration));
			_view.SetMegabotStyle(currentCpu > cpuPower.MaxCpuPower);
			_view.SetCurrentCpuLabel(currentCpu.ToString("N0"));
			_view.SetCurrentCpuSliderInvalidState(_localCurrentCpu > cpuPower.MaxMegabotCpuPower);
		}

		private void SetVisibleCurrentCpu(float cpu)
		{
			if (cpu > (float)(double)cpuPower.MaxCpuPower)
			{
				_view.CurrentCpuSliderValue = 1f;
				_view.MegabotCurrentCpuSliderValue = GetCpuSliderValue(cpu);
			}
			else
			{
				_view.CurrentCpuSliderValue = GetCpuSliderValue(cpu);
				_view.MegabotCurrentCpuSliderValue = 0f;
			}
			_visibleCurrentCpu = cpu;
		}

		private float GetCpuSliderValue(float cpu)
		{
			if (cpu > (float)(double)cpuPower.MaxCpuPower)
			{
				return (cpu - (float)(double)cpuPower.MaxCpuPower) / (float)(double)(cpuPower.MaxMegabotCpuPower - cpuPower.MaxCpuPower);
			}
			return cpu / (float)(double)cpuPower.MaxCpuPower;
		}
	}
}
