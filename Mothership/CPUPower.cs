using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal sealed class CPUPower : ICPUPower, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private uint _maxCosmeticCPUPool;

		private CPUSettingsDependency _cpuSettings;

		private bool _settingsLoaded;

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeTypeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
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
		internal MaxCosmeticCPUChangedObservable maxCosmeticCPUChangedObservable
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

		public uint MaxCpuPower => _cpuSettings.maxRegularCpu;

		public uint MaxMegabotCpuPower => _cpuSettings.maxMegabotCpu;

		public uint TotalActualCPUCurrentRobot => TotalCPUCurrentRobot - CurrentCosmeticCpuPool;

		public uint TotalCPUCurrentRobot
		{
			get;
			set;
		}

		public uint TotalCosmeticCPUCurrentRobot
		{
			get;
			set;
		}

		public uint MaxCosmeticCpuPool => _maxCosmeticCPUPool;

		public uint CurrentCosmeticCpuPool
		{
			get;
			set;
		}

		private event Action<uint> OnCPULoadChange = delegate
		{
		};

		private event Action<uint> OnCCPULoadChange = delegate
		{
		};

		public void OnFrameworkInitialized()
		{
			premiumMembership.onSubscriptionActivated += OnPremiumActivated;
			premiumMembership.onSubscriptionExpired += OnPremiumExpired;
			machineMap.OnAddCubeAt += OnAddCubeAt;
			machineMap.OnRemoveCubeAt += OnRemoveCubeAt;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			premiumMembership.onSubscriptionActivated -= OnPremiumActivated;
			premiumMembership.onSubscriptionExpired -= OnPremiumExpired;
			machineMap.OnAddCubeAt -= OnAddCubeAt;
			machineMap.OnRemoveCubeAt -= OnRemoveCubeAt;
		}

		public void RegisterOnCPULoadChanged(Action<uint> action)
		{
			OnCPULoadChange += action;
		}

		public void UnregisterOnCPULoadChanged(Action<uint> action)
		{
			OnCPULoadChange -= action;
		}

		public void RegisterOnCosmeticCPULoadChanged(Action<uint> action)
		{
			OnCCPULoadChange += action;
		}

		public void UnregisterOnCosmeticCPULoadChanged(Action<uint> action)
		{
			OnCCPULoadChange -= action;
		}

		public void MothershipFlowCompleted()
		{
			this.OnCPULoadChange(TotalActualCPUCurrentRobot);
			this.OnCCPULoadChange(CurrentCosmeticCpuPool);
		}

		public IEnumerator LoadData()
		{
			Debug.Log((object)"loading CPU power data");
			ILoadCpuSettingsRequest cpuRequest = serviceFactory.Create<ILoadCpuSettingsRequest>();
			TaskService<CPUSettingsDependency> task = new TaskService<CPUSettingsDependency>(cpuRequest);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadCPUPowerData");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadCPUPowerData");
			}).GetEnumerator();
			if (task.succeeded)
			{
				_cpuSettings = task.result;
				_settingsLoaded = true;
				SetCosmeticCPUPool();
			}
		}

		public IEnumerator IsLoadedEnumerator()
		{
			while (!_settingsLoaded)
			{
				yield return null;
			}
		}

		public bool IsLoaded()
		{
			return _settingsLoaded;
		}

		private void OnAddCubeAt(Byte3 gridLoc, MachineCell cell)
		{
			if (cell != null)
			{
				CubeTypeID cubeType = cell.info.persistentCubeData.cubeType;
				uint cubeCPURating = cubeTypeInventory.GetCubeCPURating(cubeType);
				if (cell.info.persistentCubeData.itemType == ItemType.Cosmetic)
				{
					TotalCosmeticCPUCurrentRobot += cubeCPURating;
				}
				TotalCPUCurrentRobot += cubeCPURating;
				CurrentCosmeticCpuPool = ((TotalCosmeticCPUCurrentRobot <= _maxCosmeticCPUPool) ? TotalCosmeticCPUCurrentRobot : _maxCosmeticCPUPool);
				this.OnCPULoadChange(TotalActualCPUCurrentRobot);
				this.OnCCPULoadChange(CurrentCosmeticCpuPool);
			}
		}

		private void OnRemoveCubeAt(Byte3 gridLoc, MachineCell cell)
		{
			if (cell != null)
			{
				CubeTypeID cubeType = cell.info.persistentCubeData.cubeType;
				uint cubeCPURating = cubeTypeInventory.GetCubeCPURating(cubeType);
				if (cell.info.persistentCubeData.itemType == ItemType.Cosmetic)
				{
					TotalCosmeticCPUCurrentRobot -= cubeCPURating;
				}
				TotalCPUCurrentRobot -= cubeCPURating;
				CurrentCosmeticCpuPool = ((TotalCosmeticCPUCurrentRobot <= _maxCosmeticCPUPool) ? TotalCosmeticCPUCurrentRobot : _maxCosmeticCPUPool);
				this.OnCPULoadChange(TotalActualCPUCurrentRobot);
				this.OnCCPULoadChange(CurrentCosmeticCpuPool);
			}
		}

		private void OnPremiumActivated(TimeSpan t)
		{
			UpdatePremiumCPUValues();
		}

		private void OnPremiumExpired()
		{
			UpdatePremiumCPUValues();
		}

		private void UpdatePremiumCPUValues()
		{
			SetCosmeticCPUPool();
			CurrentCosmeticCpuPool = ((TotalCosmeticCPUCurrentRobot <= _maxCosmeticCPUPool) ? TotalCosmeticCPUCurrentRobot : _maxCosmeticCPUPool);
			this.OnCPULoadChange(TotalActualCPUCurrentRobot);
			this.OnCCPULoadChange(CurrentCosmeticCpuPool);
			maxCosmeticCPUChangedObservable.Dispatch(ref _maxCosmeticCPUPool);
		}

		private void SetCosmeticCPUPool()
		{
			_maxCosmeticCPUPool = (premiumMembership.hasSubscription ? _cpuSettings.premiumCosmeticCPU : _cpuSettings.noPremiumCosmeticCPU);
			if (premiumMembership.hasPremiumForLife)
			{
				_maxCosmeticCPUPool = _cpuSettings.premiumForLifeCosmeticCPU;
			}
		}
	}
}
