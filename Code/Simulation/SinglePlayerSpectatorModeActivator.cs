using Simulation.DeathEffects;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal sealed class SinglePlayerSpectatorModeActivator : IInitialize, IWaitForFrameworkDestruction, ISpectatorModeActivator
	{
		private bool _gameEnded;

		[Inject]
		internal ICommandFactory commandFactory
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
		internal PlayerTeamsContainer playerTeamsContainer
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
		internal DeathAnimationFinishedObserver deathAnimationFinishedObserver
		{
			private get;
			set;
		}

		private event Action<int, bool> _onSpectatorModeActivate;

		unsafe void IInitialize.OnDependenciesInjected()
		{
			deathAnimationFinishedObserver.AddAction(new ObserverAction<Kill>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded += HandleGameEnded;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded -= HandleGameEnded;
			deathAnimationFinishedObserver.RemoveAction(new ObserverAction<Kill>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleGameEnded(bool victory)
		{
			_gameEnded = true;
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
				TaskRunner.get_Instance().Run(SkipSpectatorModeAfterDelay(3f, kill.shooterId));
			}
		}

		private IEnumerator SkipSpectatorModeAfterDelay(float delay, int shooter)
		{
			this._onSpectatorModeActivate(shooter, arg2: true);
			while (true)
			{
				float num;
				delay = (num = delay - Time.get_deltaTime());
				if (!(num > 0f))
				{
					break;
				}
				yield return null;
			}
			if (!_gameEnded)
			{
				RequestRespawnPointClientCommand requestRespawnCommand = commandFactory.Build<RequestRespawnPointClientCommand>();
				requestRespawnCommand.Execute();
				this._onSpectatorModeActivate(shooter, arg2: false);
			}
		}
	}
}
