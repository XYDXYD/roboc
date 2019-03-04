using Simulation.Hardware.Weapons;
using Simulation.TeamBuff;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Simulation
{
	internal sealed class HealthTracker : IInitialize, IWaitForFrameworkDestruction
	{
		public struct PlayerHealthChangedInfo
		{
			public int shooterId;

			public int shotPlayerId;

			public int shotMachineId;

			public int deltaHealth;

			public TargetType shooterTargetType;

			public PlayerHealthChangedInfo(int shooterId_, int shotPlayerId_, int shotmachineId_, int deltaHealth_, TargetType shooterTargetType_)
			{
				shooterId = shooterId_;
				shotPlayerId = shotPlayerId_;
				shotMachineId = shotmachineId_;
				deltaHealth = deltaHealth_;
				shooterTargetType = shooterTargetType_;
			}
		}

		private class HealthState
		{
			public int currentHealth;

			public int currentMaxHealth;

			public int initialHealth;
		}

		private readonly Dictionary<TargetType, Dictionary<int, HealthState>> _currentHealthPerTarget = new Dictionary<TargetType, Dictionary<int, HealthState>>(new TargetTypeComparer());

		private bool _isLocalMachineDestroyed;

		private float _critRatio = 2f;

		[CompilerGenerated]
		private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal HealingReporter healingReporter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher machineDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerCubesBuffedObserver playerCubesBuffedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
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

		public event Action<PlayerHealthChangedInfo> OnPlayerHealthChanged = delegate
		{
		};

		public event Action<int, int> OnPlayerHealthInit = delegate
		{
		};

		public event Action<TargetType, int, float, float> OnEntityHealthChanged = delegate
		{
		};

		public event Action<SpawnInParametersEntity, int> OnEntityHealthInit = delegate
		{
		};

		public event Action<HealthChangeData> HealthChanged = delegate
		{
		};

		public unsafe void OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			healingReporter.PlayerHealed += HandleOnPlayerCubesHealed;
			healingReporter.OnProtoniumHealingApplied += HandleOnProtoniumHealingApplied;
			machineDispatcher.OnPlayerRegistered += HandlePlayerRegistered;
			machineDispatcher.OnPlayerRespawnedIn += HandleonRespawnIn;
			machineDispatcher.OnEntitySpawnedIn += HandleOnEntitySpawnedIn;
			machineDispatcher.OnEntityRespawnedIn += HandleOnEntityRespawnedIn;
			destructionReporter.OnProtoniumDamageApplied += HandleProtoniumDamageApplied;
			playerCubesBuffedObserver.AddAction(new ObserverAction<PlayerCubesBuffedDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			LoadGameClientSettings();
		}

		public unsafe void OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			healingReporter.PlayerHealed -= HandleOnPlayerCubesHealed;
			machineDispatcher.OnPlayerRegistered -= HandlePlayerRegistered;
			machineDispatcher.OnEntitySpawnedIn -= HandleOnEntitySpawnedIn;
			machineDispatcher.OnEntityRespawnedIn -= HandleOnEntityRespawnedIn;
			destructionReporter.OnProtoniumDamageApplied -= HandleProtoniumDamageApplied;
			playerCubesBuffedObserver.RemoveAction(new ObserverAction<PlayerCubesBuffedDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public float GetCurrentHealth(TargetType type, int machineId)
		{
			HealthState healthState = _currentHealthPerTarget[type][machineId];
			return healthState.currentHealth;
		}

		public float GetCurrentHealthPercent(TargetType type, int machineId)
		{
			HealthState healthState = _currentHealthPerTarget[type][machineId];
			return (float)healthState.currentHealth / (float)healthState.currentMaxHealth;
		}

		public bool IsFullHealth(TargetType type, int machineId)
		{
			HealthState healthState = _currentHealthPerTarget[type][machineId];
			return healthState.currentHealth == healthState.currentMaxHealth;
		}

		public float HealthLost(TargetType type, int machineId)
		{
			HealthState healthState = _currentHealthPerTarget[type][machineId];
			return healthState.currentMaxHealth - healthState.currentHealth;
		}

		public bool IsDead(TargetType type, int machineId)
		{
			HealthState healthState = _currentHealthPerTarget[type][machineId];
			return healthState.currentHealth == 0;
		}

		public int GetMaxHealth(TargetType type, int playerId)
		{
			HealthState healthState = _currentHealthPerTarget[type][playerId];
			return healthState.currentMaxHealth;
		}

		public void ApplyCampaignHealthBoost(int machineId, int health)
		{
			_currentHealthPerTarget[TargetType.Player][machineId].currentHealth = health;
			_currentHealthPerTarget[TargetType.Player][machineId].currentMaxHealth = health;
			_currentHealthPerTarget[TargetType.Player][machineId].initialHealth = health;
		}

		internal void SetEqualizerHealthAfterReconnect(int health, int maxHealth)
		{
			HealthState healthState = _currentHealthPerTarget[TargetType.EqualizerCrystal][0];
			healthState.currentHealth = health;
			healthState.initialHealth = maxHealth;
			healthState.currentMaxHealth = maxHealth;
		}

		private void HandleOnProtoniumHealingApplied(int hitId, FasterList<InstantiatedCube> respawnedCubes, FasterList<InstantiatedCube> healedCubes, TargetType type)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			FasterListEnumerator<InstantiatedCube> enumerator = respawnedCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.get_Current();
					num += current.lastHealingApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			FasterListEnumerator<InstantiatedCube> enumerator2 = healedCubes.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InstantiatedCube current2 = enumerator2.get_Current();
					num += current2.lastHealingApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			HealthState healthState = _currentHealthPerTarget[type][hitId];
			healthState.currentHealth += num;
			float arg = (float)num / (float)healthState.currentMaxHealth;
			this.OnEntityHealthChanged(type, hitId, (float)healthState.currentHealth / (float)healthState.currentMaxHealth, arg);
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			if (data.targetIsMe && data.isDestroyed)
			{
				if (_isLocalMachineDestroyed)
				{
					return;
				}
				_isLocalMachineDestroyed = true;
			}
			int num = 0;
			FasterListEnumerator<InstantiatedCube> enumerator = data.destroyedCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.get_Current();
					num += current.lastDamageApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			FasterListEnumerator<InstantiatedCube> enumerator2 = data.damagedCubes.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InstantiatedCube current2 = enumerator2.get_Current();
					num += current2.lastDamageApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			_currentHealthPerTarget[data.targetType][data.hitMachineId].currentHealth -= num;
			PlayerHealthChangedInfo obj = new PlayerHealthChangedInfo(data.shooterId, data.hitPlayerId, data.hitMachineId, -num, TargetType.Player);
			this.OnPlayerHealthChanged(obj);
			HealthChangeData healthChangeData = default(HealthChangeData);
			healthChangeData.shooterType = TargetType.Player;
			healthChangeData.shooterId = data.shooterId;
			healthChangeData.shooterIsMe = data.shooterIsMe;
			healthChangeData.targetType = data.targetType;
			healthChangeData.targetId = data.hitMachineId;
			healthChangeData.targetIsMe = data.targetIsMe;
			healthChangeData.deltaHealth = -num;
			healthChangeData.isTargetDead = data.isDestroyed;
			healthChangeData.IsCriticalHit = ((float)num > (float)data.weaponDamage * _critRatio);
			HealthChangeData obj2 = healthChangeData;
			this.HealthChanged(obj2);
		}

		private void HandleOnEntityRespawnedIn(SpawnInParametersEntity spawnInParameters)
		{
			HealthState healthState = _currentHealthPerTarget[spawnInParameters.Type][spawnInParameters.EntityID];
			healthState.currentHealth = healthState.initialHealth;
			healthState.currentMaxHealth = healthState.initialHealth;
			this.OnEntityHealthInit(spawnInParameters, healthState.initialHealth);
		}

		private void HandleProtoniumDamageApplied(DestructionData data)
		{
			ComputeDamage(data.targetType, data.hitMachineId, data.destroyedCubes, data.damagedCubes);
		}

		private void ComputeDamage(TargetType type, int hitId, FasterList<InstantiatedCube> destroyedCubes, FasterList<InstantiatedCube> damagedCubes)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			FasterListEnumerator<InstantiatedCube> enumerator = destroyedCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.get_Current();
					num += current.lastDamageApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			FasterListEnumerator<InstantiatedCube> enumerator2 = damagedCubes.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InstantiatedCube current2 = enumerator2.get_Current();
					num += current2.lastDamageApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			HealthState healthState = _currentHealthPerTarget[type][hitId];
			healthState.currentHealth -= num;
			float arg = (float)(-num) / (float)healthState.currentMaxHealth;
			this.OnEntityHealthChanged(type, hitId, (float)healthState.currentHealth / (float)healthState.currentMaxHealth, arg);
		}

		private void HandleOnEntitySpawnedIn(SpawnInParametersEntity spawnInParameters)
		{
			if (spawnInParameters.Type != 0)
			{
				if (!_currentHealthPerTarget.ContainsKey(spawnInParameters.Type))
				{
					_currentHealthPerTarget.Add(spawnInParameters.Type, new Dictionary<int, HealthState>());
				}
				HealthState healthState = new HealthState();
				if (spawnInParameters.Type == TargetType.TeamBase)
				{
					healthState.currentHealth = 0;
					healthState.currentMaxHealth = (spawnInParameters.PreloadedMachine.batchableCubes.get_Count() - 1) * spawnInParameters.BaseData.protoniumHealth;
					healthState.initialHealth = 0;
				}
				else if (spawnInParameters.Type == TargetType.EqualizerCrystal)
				{
					healthState.currentHealth = spawnInParameters.BaseData.equalizerHealth;
					healthState.currentMaxHealth = spawnInParameters.BaseData.equalizerHealth;
					healthState.initialHealth = spawnInParameters.BaseData.equalizerHealth;
				}
				_currentHealthPerTarget[spawnInParameters.Type][spawnInParameters.EntityID] = healthState;
				this.OnEntityHealthInit(spawnInParameters, healthState.initialHealth);
			}
		}

		private void HandleonRespawnIn(SpawnInParametersPlayer spawnInParameters)
		{
			HealthState healthState = _currentHealthPerTarget[TargetType.Player][spawnInParameters.playerId];
			healthState.currentHealth = healthState.initialHealth;
			healthState.currentMaxHealth = healthState.initialHealth;
			this.OnPlayerHealthInit(spawnInParameters.playerId, healthState.initialHealth);
			_isLocalMachineDestroyed = false;
		}

		private void HandleOnPlayerCubesHealed(HealingData data)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			FasterListEnumerator<InstantiatedCube> enumerator = data.healedCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.get_Current();
					num += current.lastHealingApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			FasterListEnumerator<InstantiatedCube> enumerator2 = data.respawnedCubes.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InstantiatedCube current2 = enumerator2.get_Current();
					num += current2.lastHealingApplied;
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			_currentHealthPerTarget[TargetType.Player][data.targetId].currentHealth += num;
			PlayerHealthChangedInfo obj = new PlayerHealthChangedInfo(data.shooterPlayerId, data.targetId, data.hitMachineId, num, TargetType.Player);
			this.OnPlayerHealthChanged(obj);
			HealthChangeData healthChangeData = default(HealthChangeData);
			healthChangeData.shooterType = data.shooterType;
			healthChangeData.shooterId = data.shooterPlayerId;
			healthChangeData.hitMachineId = data.hitMachineId;
			healthChangeData.shooterIsMe = playerTeamsContainer.IsMe(data.shooterType, data.shooterPlayerId);
			healthChangeData.targetType = TargetType.Player;
			healthChangeData.targetId = data.targetId;
			healthChangeData.targetIsMe = playerTeamsContainer.IsMe(TargetType.Player, data.targetId);
			healthChangeData.deltaHealth = num;
			healthChangeData.isTargetDead = false;
			healthChangeData.IsCriticalHit = false;
			HealthChangeData obj2 = healthChangeData;
			this.HealthChanged(obj2);
		}

		private void HandlePlayerRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			if (!_currentHealthPerTarget.ContainsKey(TargetType.Player))
			{
				_currentHealthPerTarget.Add(TargetType.Player, new Dictionary<int, HealthState>());
			}
			int machineId = spawnInParameters.preloadedMachine.machineId;
			int totalHealth = spawnInParameters.preloadedMachine.machineInfo.totalHealth;
			int currentHealth = totalHealth;
			HealthState healthState = new HealthState();
			healthState.currentHealth = currentHealth;
			healthState.currentMaxHealth = totalHealth;
			healthState.initialHealth = totalHealth;
			_currentHealthPerTarget[TargetType.Player][machineId] = healthState;
			this.OnPlayerHealthInit(machineId, totalHealth);
		}

		private void UpdatePlayerHealth(ref PlayerCubesBuffedDependency dependency)
		{
			HealthState healthState = _currentHealthPerTarget[TargetType.Player][dependency.machineId];
			healthState.currentHealth = dependency.currentHealth;
			healthState.initialHealth = (healthState.currentMaxHealth = dependency.initialTotalHealth);
		}

		private void LoadGameClientSettings()
		{
			IGetGameClientSettingsRequest getGameClientSettingsRequest = serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_critRatio = data.criticalRatio;
			}, ErrorWindow.ShowServiceErrorWindow));
			getGameClientSettingsRequest.Execute();
		}
	}
}
