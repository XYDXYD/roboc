using DG.Tweening;
using DG.Tweening.Core;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;

namespace Mothership.GUI
{
	internal class HUDDamageBoostPresenter : IWaitForFrameworkDestruction
	{
		private uint _totalCPU;

		private DamageBoostUtility _damageBoostUtility;

		private Sequence _mainAnimationSequence;

		private HUDDamageBoostView _view;

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		public void Initialise()
		{
			ClearValues();
		}

		public IEnumerator LoadGUIData()
		{
			loadingIconPresenter.NotifyLoading("DamageBoostSettings");
			IGetDamageBoostRequest damageBoostRequest = serviceFactory.Create<IGetDamageBoostRequest>();
			TaskService<DamageBoostDeserialisedData> damageBoostRequestTask = new TaskService<DamageBoostDeserialisedData>(damageBoostRequest);
			yield return new HandleTaskServiceWithError(damageBoostRequestTask, delegate
			{
				loadingIconPresenter.NotifyLoading("DamageBoostSettings");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("DamageBoostSettings");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("DamageBoostSettings");
			_damageBoostUtility = new DamageBoostUtility((DamageBoostDeserialisedData)damageBoostRequestTask.result.Clone());
			RegisterEventListeners();
		}

		private void RegisterEventListeners()
		{
			cpuPower.RegisterOnCPULoadChanged(OnCurrentCpuLoadChange);
		}

		internal void ForceDisplayedDamageBoostToZero(bool setToZero)
		{
			if (setToZero)
			{
				UnregisterEventListeners();
				DOTween.Kill((object)this, false);
				ClearValues();
			}
			else
			{
				RegisterEventListeners();
			}
		}

		public void OnFrameworkDestroyed()
		{
			UnregisterEventListeners();
		}

		private void UnregisterEventListeners()
		{
			cpuPower.UnregisterOnCPULoadChanged(OnCurrentCpuLoadChange);
		}

		internal void SetView(HUDDamageBoostView hUDHealthView)
		{
			_view = hUDHealthView;
		}

		private void ClearValues()
		{
			_totalCPU = 0u;
			_view.SliderValue = 0f;
			_view.SetValueLabel("0.00%");
			_view.SetHeaderLabel(StringTableBase<StringTable>.Instance.GetString("strDamageBoost"));
		}

		private void OnCurrentCpuLoadChange(uint _currentCpu)
		{
			_totalCPU = _currentCpu;
			AnimateBar();
		}

		private unsafe void AnimateBar()
		{
			float num = CurrentRobotDamageBoostPercentage();
			string valueLabel = $"{num * 100f:0.00}%";
			_view.SetValueLabel(valueLabel);
			TweenExtensions.Kill(_mainAnimationSequence, false);
			_mainAnimationSequence = DOTween.Sequence();
			_mainAnimationSequence.target = this;
			TweenSettingsExtensions.Append(_mainAnimationSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), num, _view.tweenDuration));
		}

		private float CurrentRobotDamageBoostPercentage()
		{
			return _damageBoostUtility.CurrentRobotDamageBoostPercentage(_totalCPU);
		}
	}
}
