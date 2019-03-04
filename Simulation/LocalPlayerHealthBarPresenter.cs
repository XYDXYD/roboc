using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class LocalPlayerHealthBarPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private LocalPlayerHealthBarView _playerHealthBarView;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineCpuDataManager cpuManager
		{
			private get;
			set;
		}

		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			cpuManager.OnMachineCpuInitialized += HandleOnMachineCpuInitialized;
			cpuManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
			spectatorModeActivator.Register(SpectatorModeToggled);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			cpuManager.OnMachineCpuInitialized -= HandleOnMachineCpuInitialized;
			cpuManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
			spectatorModeActivator.Unregister(SpectatorModeToggled);
		}

		public void Register(LocalPlayerHealthBarView playerHealthBarView)
		{
			_playerHealthBarView = playerHealthBarView;
		}

		public void Unregister(LocalPlayerHealthBarView playerHealthBarView)
		{
			_playerHealthBarView = null;
		}

		private void SpectatorModeToggled(int myKiller, bool enabled)
		{
			_playerHealthBarView.Enable(!enabled);
		}

		private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int hitMachineId, float percent)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, hitMachineId);
			if (playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
			{
				_playerHealthBarView.UpdateHealth(percent);
			}
		}

		private void HandleOnMachineCpuInitialized(int machineId, uint health)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
			{
				_playerHealthBarView.InitializeHealth();
			}
		}
	}
}
