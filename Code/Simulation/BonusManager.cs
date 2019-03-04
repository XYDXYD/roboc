using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System.Collections;

namespace Simulation
{
	internal class BonusManager : IInitialize, IWaitForFrameworkDestruction
	{
		private AwardProtoniumDestroyedCubesRequestDependency _awardProtoniumDestroyedCubesRequestDependency;

		private bool _canSwitchToMothership;

		[Inject]
		internal IDispatchWorldSwitching worldSwitch
		{
			get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			get;
			set;
		}

		[Inject]
		public HealingReporter healingReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			get;
			set;
		}

		[Inject]
		public MachineSpawnDispatcher machineSpawnDispatcher
		{
			get;
			set;
		}

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			get;
			set;
		}

		[Inject]
		public LocalAIsContainer localAIs
		{
			private get;
			set;
		}

		[Inject]
		public ProtectTeamMateBonusManager protectTeamMateBonusManager
		{
			private get;
			set;
		}

		[Inject]
		public DefendTheBaseBonusManager defendTheBaseBonusManager
		{
			private get;
			set;
		}

		[Inject]
		public HealCubesBonusManager healCubesBonusManager
		{
			private get;
			set;
		}

		[Inject]
		public RoboPointAwardManager roboPointAwardManager
		{
			get;
			set;
		}

		public virtual void OnDependenciesInjected()
		{
			worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
			_awardProtoniumDestroyedCubesRequestDependency = new AwardProtoniumDestroyedCubesRequestDependency(0);
			destructionReporter.OnPlayerDamageApplied += HandleOnLocalPlayerCubesDestroyed;
			healingReporter.OnPlayerCubesRespawned += HandleOnPlayerCubesRespawned;
			machineSpawnDispatcher.OnPlayerRespawnedIn += HandleOnRespawnedIn;
			destructionReporter.OnProtoniumDamageApplied += HandleOnProtoniumCubesDestroyed;
		}

		public virtual void OnFrameworkDestroyed()
		{
			worldSwitch.OnWorldJustSwitched -= HandleOnWorldJustSwitched;
			destructionReporter.OnPlayerDamageApplied -= HandleOnLocalPlayerCubesDestroyed;
			healingReporter.OnPlayerCubesRespawned -= HandleOnPlayerCubesRespawned;
			machineSpawnDispatcher.OnPlayerRespawnedIn -= HandleOnRespawnedIn;
			destructionReporter.OnProtoniumDamageApplied -= HandleOnProtoniumCubesDestroyed;
		}

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			if (obj == WorldSwitchMode.SimulationMP)
			{
				worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
			}
		}

		public void IgnoreReplyFromGameServer()
		{
			_canSwitchToMothership = true;
		}

		public void BonusSaveComplete()
		{
			_canSwitchToMothership = true;
		}

		public void ConnectionError()
		{
			_canSwitchToMothership = true;
		}

		protected IEnumerator OnPreSwitchWorld()
		{
			while (!_canSwitchToMothership)
			{
				yield return null;
			}
		}

		private void HandleOnRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isLocal)
			{
				roboPointAwardManager.ResetAwardedBonuses(spawnInParameters.playerId);
				protectTeamMateBonusManager.ResetAwardedBonuses(spawnInParameters.playerId);
				defendTheBaseBonusManager.ResetAwardedBonuses(spawnInParameters.playerId);
			}
		}

		private void HandleOnProtoniumCubesDestroyed(DestructionData data)
		{
			if (data.shooterIsMe && data.destroyedCubes.get_Count() > 0)
			{
				NetworkEvent type = NetworkEvent.AwardTeamBaseProtoniumDestroyedRequest;
				_awardProtoniumDestroyedCubesRequestDependency.SetDestroyedCubesCount(data.destroyedCubes.get_Count());
				_awardProtoniumDestroyedCubesRequestDependency.SetId(data.hitPlayerId);
				_awardProtoniumDestroyedCubesRequestDependency.SetProtoniumCubeId(data.destroyedCubes.get_Item(0).persistentCubeData.cubeType.ID);
				eventManagerClient.SendEventToServer(type, _awardProtoniumDestroyedCubesRequestDependency);
			}
		}

		private void HandleOnLocalPlayerCubesDestroyed(DestructionData data)
		{
			if (data.targetIsLocal && data.shooterId != data.hitPlayerId)
			{
				roboPointAwardManager.ProcessLocalPlayerCubesDestroyed(data.shooterId, data.hitPlayerId, data.destroyedCubes, data.isDestroyed);
				defendTheBaseBonusManager.ProcessLocalCubesDestroyed(data.shooterId, data.hitPlayerId, data.destroyedCubes, data.isDestroyed);
			}
		}

		private void HandleOnPlayerCubesRespawned(int shooterId, int healedMachineId, FasterList<InstantiatedCube> respawnedCubes, TargetType shooterTargetType)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, healedMachineId);
			if ((localAIs.IsAIHostedLocally(playerFromMachineId) || playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId)) && playerFromMachineId != shooterId && shooterTargetType == TargetType.Player)
			{
				healCubesBonusManager.ProcessLocalPlayerCubesHealed(shooterId, playerFromMachineId, respawnedCubes);
			}
		}
	}
}
