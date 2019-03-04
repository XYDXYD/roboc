using DG.Tweening;
using DG.Tweening.Core;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDHealthPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private const int MULTIPLIER = 10000000;

		private int _totalHealth;

		private float _totalHealthBoost;

		private int _displayedHealthBoost;

		private float _displayedHealth;

		private float _maxHealth;

		private float _maxMegabotHealth;

		private float _maxHealthBoost = 1f;

		private Sequence _mainHealthSequence;

		private Sequence _mainBoostSequence;

		private HUDHealthView _view;

		[Inject]
		internal IMachineMap machineMap
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

		public void OnDependenciesInjected()
		{
			RegisterEventListeners();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadCPUSettings);
		}

		public void Initialise()
		{
			ClearValues();
		}

		private void RegisterEventListeners()
		{
			machineMap.OnAddCubeAt += HandleCubePlaced;
			machineMap.OnRemoveCubeAt += HandleOnCubeRemoved;
		}

		internal void ForceDisplayedHealthToZero(bool lockToZero)
		{
			if (lockToZero)
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
			machineMap.OnAddCubeAt -= HandleCubePlaced;
			machineMap.OnRemoveCubeAt -= HandleOnCubeRemoved;
		}

		internal void SetView(HUDHealthView hUDHealthView)
		{
			_view = hUDHealthView;
			_maxHealthBoost = _view.MaxHealthBoost;
		}

		private IEnumerator LoadCPUSettings()
		{
			TaskService<CPUSettingsDependency> loadCPUTaskService = serviceFactory.Create<ILoadCpuSettingsRequest>().AsTask();
			yield return new HandleTaskServiceWithError(loadCPUTaskService, null, null).GetEnumerator();
			CPUSettingsDependency cpuSettings = loadCPUTaskService.result;
			_maxHealth = (float)(double)cpuSettings.maxRegularHealth;
			_maxMegabotHealth = (float)(double)cpuSettings.maxMegabotHealth;
		}

		private void ClearValues()
		{
			_view.HealthSliderValue = 0f;
			_view.HealthBoostSliderValue = 0f;
			_view.HealthLabel.set_text("0");
			_view.HealthBoostLabel.set_text("0.00%");
		}

		private void HandleOnCubeRemoved(Byte3 arg1, MachineCell cell)
		{
			PersistentCubeData persistentCubeData = cell.info.persistentCubeData;
			if (persistentCubeData.itemType != ItemType.Cosmetic)
			{
				UpdateHealth(-persistentCubeData.health, 0f - persistentCubeData.healthBoost);
			}
		}

		private void HandleCubePlaced(Byte3 arg1, MachineCell cell)
		{
			PersistentCubeData persistentCubeData = cell.info.persistentCubeData;
			if (persistentCubeData.itemType != ItemType.Cosmetic)
			{
				UpdateHealth(persistentCubeData.health, persistentCubeData.healthBoost);
			}
		}

		private void UpdateHealth(int deltaHealth, float deltaHealthBoost)
		{
			_totalHealth += deltaHealth;
			AnimateHealthPrimaryBar();
			_view.HealthLabel.set_text(_totalHealth.ToString("N0"));
			_totalHealthBoost += deltaHealthBoost;
			_displayedHealthBoost += (int)(deltaHealthBoost * 1E+07f);
			AnimateHealthBoostPrimaryBar();
			_view.HealthBoostLabel.set_text(((float)_displayedHealthBoost / 1E+07f).ToString("P", CultureInfo.InvariantCulture));
		}

		private unsafe void AnimateHealthPrimaryBar()
		{
			TweenExtensions.Kill(_mainHealthSequence, false);
			_mainHealthSequence = DOTween.Sequence();
			TweenSettingsExtensions.SetRecyclable<Sequence>(_mainHealthSequence, false);
			TweenSettingsExtensions.Append(_mainHealthSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), (float)_totalHealth, _view.tweenDuration));
		}

		private void SetVisibleCurrentHealth(float health)
		{
			if (health > _maxHealth)
			{
				_view.HealthSliderValue = 1f;
				_view.MegabotHealthSliderValue = GetHealtSliderValue(health);
			}
			else
			{
				_view.HealthSliderValue = GetHealtSliderValue(health);
				_view.MegabotHealthSliderValue = 0f;
			}
			_displayedHealth = health;
		}

		private float GetHealtSliderValue(float health)
		{
			if (health > _maxHealth)
			{
				return (health - _maxHealth) / (_maxMegabotHealth - _maxHealth);
			}
			return health / _maxHealth;
		}

		private unsafe void AnimateHealthBoostPrimaryBar()
		{
			float num = Mathf.Clamp01(_totalHealthBoost / _maxHealthBoost);
			TweenExtensions.Kill(_mainBoostSequence, false);
			_mainBoostSequence = DOTween.Sequence();
			_mainBoostSequence.target = this;
			TweenSettingsExtensions.SetRecyclable<Sequence>(_mainBoostSequence, false);
			TweenSettingsExtensions.Append(_mainBoostSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), num, _view.tweenDuration));
		}
	}
}
