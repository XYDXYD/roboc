using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class ClassicModeSpectatorModeActivator : IInitialize, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization, ISpectatorModeActivator
	{
		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal LivePlayersContainer livePlayersContainer
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

		private event Action<int, bool> _onSpectatorModeActivate;

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += OnMachineKilled;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			GameObject val = factory.Build("Spectator");
			val.SetActive(false);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= OnMachineKilled;
		}

		public void Register(Action<int, bool> onActivate)
		{
			_onSpectatorModeActivate += onActivate;
		}

		public void Unregister(Action<int, bool> onActivate)
		{
			_onSpectatorModeActivate -= onActivate;
		}

		private void OnMachineKilled(int owner, int shooter)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, owner) && OtherPlayersAreAliveOnMyTeam())
			{
				ActivateSpectatorMode(shooter, enabled: true);
			}
		}

		private bool OtherPlayersAreAliveOnMyTeam()
		{
			IList<int> livePlayers = livePlayersContainer.GetLivePlayers(TargetType.Player);
			for (int i = 0; i < livePlayers.Count; i++)
			{
				if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, livePlayers[i]))
				{
					return true;
				}
			}
			return false;
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
