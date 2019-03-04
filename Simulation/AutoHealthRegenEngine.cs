using Simulation.Hardware.Modules.Emp.Observers;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class AutoHealthRegenEngine : IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const int HEAL_LIMIT = 10000000;

		private const float _healthRestoreInterval = 0.1f;

		private List<HitCubeInfo> _proposedResult = new List<HitCubeInfo>();

		private bool _enableAutoHeal = true;

		private float _secondsToWaitHeal = 2f;

		private float _secondsToFullHeal = 3f;

		private float _secondsDurationOfSpawnHeal = 20f;

		private float _secondsToSpawnFullHeal = 1f;

		[Inject]
		internal ICommandFactory commandFactory
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
		internal CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTracker
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
		internal MachineStunnedObserver machineStunnedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal RespawnHealthSettingsObserver respawnHealthSettingsObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run(Tick());
		}

		public unsafe void OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
			machineDispatcher.OnPlayerRespawnedIn += HandleOnSpawnedIn;
			respawnHealthSettingsObserver.OnRespawnSettingsReceived += OnRespawnSettingsReceived;
			IGetAutoRegenHealthSettings getAutoRegenHealthSettings = serviceFactory.Create<IGetAutoRegenHealthSettings>();
			getAutoRegenHealthSettings.SetAnswer(new ServiceAnswer<AutoRegenHealthSettingsData>(OnSettingsLoaded, OnFailed));
			getAutoRegenHealthSettings.Execute();
			machineStunnedObserver.AddAction(new ObserverAction<MachineStunnedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			machineDispatcher.OnPlayerRespawnedIn -= HandleOnSpawnedIn;
			respawnHealthSettingsObserver.OnRespawnSettingsReceived -= OnRespawnSettingsReceived;
			machineStunnedObserver.RemoveAction(new ObserverAction<MachineStunnedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded -= OnGameEnded;
		}

		private void OnSettingsLoaded(AutoRegenHealthSettingsData settingsData)
		{
			_secondsToWaitHeal = settingsData.secondsToWaitForHeal;
			_secondsToFullHeal = settingsData.secondsToFullHeal;
			_enableAutoHeal = settingsData.enableAutoHeal;
		}

		private void OnRespawnSettingsReceived(float respawnHealDuration, float respawnFullHealDuration)
		{
			_secondsDurationOfSpawnHeal = respawnHealDuration;
			_secondsToSpawnFullHeal = respawnFullHealDuration;
		}

		private void OnFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		private IEnumerator Tick()
		{
			while (_enableAutoHeal)
			{
				int viewsCount = default(int);
				AutoHealEntityView[] views = entityViewsDB.QueryEntityViewsAsArray<AutoHealEntityView>(ref viewsCount);
				for (int i = 0; i < viewsCount; i++)
				{
					TickMachine(views[i], Time.get_deltaTime());
				}
				yield return null;
			}
		}

		private void TickMachine(AutoHealEntityView view, float deltaSec)
		{
			IAutoHealComponent autoHealComponent = view.autoHealComponent;
			autoHealComponent.timer -= deltaSec;
			if (autoHealComponent.spawnHealTimer > 0f)
			{
				autoHealComponent.spawnHealTimer -= deltaSec;
			}
			if (autoHealComponent.spawnHealTimer > 0f || autoHealComponent.timer <= 0f)
			{
				int ownerId = view.ownerComponent.ownerId;
				int ownerMachineId = view.ownerComponent.ownerMachineId;
				if (!healthTracker.IsFullHealth(TargetType.Player, ownerMachineId))
				{
					RestoreLocalHealth(deltaSec, view);
					return;
				}
				autoHealComponent.amountOfHealthToRestore = 0;
				autoHealComponent.healTimer = 0f;
			}
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			AutoHealEntityView view = default(AutoHealEntityView);
			if (entityViewsDB.TryQueryEntityView<AutoHealEntityView>(machineId, ref view))
			{
				ResetAutoHeal(view);
			}
		}

		private void HandleOnSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			AutoHealEntityView autoHealEntityView = default(AutoHealEntityView);
			if (entityViewsDB.TryQueryEntityView<AutoHealEntityView>(spawnInParameters.machineId, ref autoHealEntityView))
			{
				if (autoHealEntityView.ownerComponent.ownedByAi && WorldSwitching.GetGameModeType() == GameModeType.Campaign)
				{
					autoHealEntityView.autoHealComponent.spawnHealTimer = 0f;
				}
				else
				{
					autoHealEntityView.autoHealComponent.spawnHealTimer = _secondsDurationOfSpawnHeal;
				}
			}
		}

		private void HandleMachineStunned(ref MachineStunnedData data)
		{
			AutoHealEntityView view = default(AutoHealEntityView);
			if (data.isStunned && entityViewsDB.TryQueryEntityView<AutoHealEntityView>(data.machineId, ref view))
			{
				ResetAutoHeal(view);
			}
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			AutoHealEntityView view = default(AutoHealEntityView);
			if (entityViewsDB.TryQueryEntityView<AutoHealEntityView>(data.hitMachineId, ref view))
			{
				ResetAutoHeal(view);
			}
		}

		private void ResetAutoHeal(AutoHealEntityView view)
		{
			if (_enableAutoHeal)
			{
				IAutoHealComponent autoHealComponent = view.autoHealComponent;
				autoHealComponent.timer = _secondsToWaitHeal;
				autoHealComponent.amountOfHealthToRestore = 0;
				autoHealComponent.healTimer = 0f;
				view.autoHealComponent.healCancelled.set_value(true);
			}
		}

		private void RestoreLocalHealth(float deltaSec, AutoHealEntityView view)
		{
			IAutoHealComponent autoHealComponent = view.autoHealComponent;
			if (autoHealComponent.healTimer <= 0f)
			{
				autoHealComponent.healTimer = ((!(autoHealComponent.spawnHealTimer > 0f)) ? _secondsToFullHeal : _secondsToSpawnFullHeal);
				autoHealComponent.timeSinceLastHealthRestore = 0f;
			}
			else
			{
				autoHealComponent.healTimer -= deltaSec;
			}
			autoHealComponent.healTimer = Math.Max(autoHealComponent.healTimer, 0.01f);
			autoHealComponent.timeSinceLastHealthRestore += deltaSec;
			if (autoHealComponent.timeSinceLastHealthRestore >= 0.1f)
			{
				int ownerMachineId = view.ownerComponent.ownerMachineId;
				float num = healthTracker.HealthLost(TargetType.Player, ownerMachineId);
				float num2 = autoHealComponent.healTimer / autoHealComponent.timeSinceLastHealthRestore;
				autoHealComponent.amountOfHealthToRestore = (int)Math.Ceiling(num / num2);
				autoHealComponent.amountOfHealthToRestore = (int)Math.Ceiling(Math.Min(autoHealComponent.amountOfHealthToRestore, num));
				RestoreMachineHealth(ownerMachineId, autoHealComponent.amountOfHealthToRestore);
				autoHealComponent.timeSinceLastHealthRestore = 0f;
			}
		}

		private void RestoreMachineHealth(int healedMachineId, int amountOfHealthToRestore)
		{
			_proposedResult.Clear();
			healingPropagator.ComputeProposedAutoHeal(healedMachineId, (amountOfHealthToRestore <= 10000000) ? amountOfHealthToRestore : 10000000, ref _proposedResult);
			if (_proposedResult.Count > 0)
			{
				HealedCubesDependency dependency = new HealedCubesDependency(healedMachineId, _proposedResult, TargetType.Player, TargetType.Player);
				HealSelfCommand healSelfCommand = commandFactory.Build<HealSelfCommand>();
				healSelfCommand.Inject(dependency);
				healSelfCommand.Execute();
			}
		}

		private void OnGameEnded(bool won)
		{
			_enableAutoHeal = false;
		}
	}
}
