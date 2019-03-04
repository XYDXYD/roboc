using Fabric;
using Simulation.DeathEffects;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation
{
	internal class SpectatorModeActivator : IInitialize, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization, ISpectatorModeActivator
	{
		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
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
		internal IGameObjectFactory factory
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
		internal DeathAnimationFinishedObserver deathAnimationFinishedObserver
		{
			private get;
			set;
		}

		private event Action<int, bool> _onSpectatorModeActivate;

		unsafe void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerRespawnedIn += HandleonRespawnedIn;
			deathAnimationFinishedObserver.AddAction(new ObserverAction<Kill>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public virtual void OnFrameworkInitialized()
		{
			GameObject val = factory.Build("Respawn_Spectator");
			val.SetActive(false);
		}

		private void HandleonRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, spawnInParameters.playerId))
			{
				ActivateSpectatorMode(spawnInParameters.playerId, enabled: false);
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			spawnDispatcher.OnPlayerRespawnedIn -= HandleonRespawnedIn;
			deathAnimationFinishedObserver.AddAction(new ObserverAction<Kill>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Register(Action<int, bool> onActivate)
		{
			_onSpectatorModeActivate += onActivate;
		}

		public void Unregister(Action<int, bool> onActivate)
		{
			_onSpectatorModeActivate -= onActivate;
		}

		private void OnMachineKilled(ref Kill kill)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, kill.victimId))
			{
				int num = kill.shooterId;
				if (!playerMachinesContainer.HasPlayerRegisteredMachine(TargetType.Player, num))
				{
					num = playerTeamsContainer.localPlayerId;
				}
				ActivateSpectatorMode(num, enabled: true);
			}
		}

		private void ActivateSpectatorMode(int myKiller, bool enabled)
		{
			if (this._onSpectatorModeActivate != null)
			{
				this._onSpectatorModeActivate(myKiller, enabled);
				AudioFabricGameEvents eventEnum = (!enabled) ? AudioFabricGameEvents.ToolTipDisappear : AudioFabricGameEvents.ToolTipAppear;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(eventEnum), 0);
			}
		}
	}
}
