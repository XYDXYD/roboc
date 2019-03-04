using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal class BonusManagerPit : BonusManager, IInitialize, IWaitForFrameworkDestruction
	{
		public override void OnDependenciesInjected()
		{
			base.worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
			base.destructionReporter.OnPlayerDamageApplied += HandleOnLocalPlayerCubesDestroyed;
			base.machineSpawnDispatcher.OnPlayerRespawnedIn += HandleOnRespawnedIn;
		}

		public override void OnFrameworkDestroyed()
		{
			base.worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
			base.destructionReporter.OnPlayerDamageApplied -= HandleOnLocalPlayerCubesDestroyed;
			base.machineSpawnDispatcher.OnPlayerRespawnedIn -= HandleOnRespawnedIn;
		}

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			base.worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
		}

		private void HandleOnRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (base.playerTeamsContainer.IsMe(TargetType.Player, spawnInParameters.playerId))
			{
				base.roboPointAwardManager.ResetAwardedBonuses(spawnInParameters.playerId);
			}
		}

		private void HandleOnLocalPlayerCubesDestroyed(DestructionData data)
		{
			if (data.targetIsMe)
			{
				base.roboPointAwardManager.ProcessLocalPlayerCubesDestroyed(data.shooterId, data.hitPlayerId, data.destroyedCubes, data.isDestroyed);
			}
		}
	}
}
