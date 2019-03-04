using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal sealed class HUDWaitTimePresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private ITaskRoutine _task;

		private bool _gameEnded;

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
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		internal HUDWaitTimeView view
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerRespawnScheduled += HandleOnRespawnScheduled;
			spawnDispatcher.OnPlayerRespawnedIn += HandleonRespawnedIn;
			spawnDispatcher.OnLocalPlayerReadyToRespawn += ResetTimer;
			gameEndedObserver.OnGameEnded += HandleOnGameEnded;
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine();
		}

		public void OnFrameworkDestroyed()
		{
			spawnDispatcher.OnPlayerRespawnScheduled -= HandleOnRespawnScheduled;
			spawnDispatcher.OnPlayerRespawnedIn -= HandleonRespawnedIn;
			spawnDispatcher.OnLocalPlayerReadyToRespawn -= ResetTimer;
			gameEndedObserver.OnGameEnded -= HandleOnGameEnded;
		}

		private void HandleOnGameEnded(bool _)
		{
			if (_task != null)
			{
				_task.Stop();
			}
			view.get_gameObject().SetActive(false);
			_gameEnded = true;
		}

		private void HandleonRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, spawnInParameters.playerId))
			{
				view.get_gameObject().SetActive(false);
			}
		}

		private void HandleOnRespawnScheduled(int playerId, int seconds)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, playerId) && !_gameEnded)
			{
				_task.SetEnumeratorProvider((Func<IEnumerator>)(() => UpdateTime(seconds))).Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator UpdateTime(int seconds)
		{
			view.get_gameObject().SetActive(true);
			float timer = seconds;
			int lastSecond = seconds + 1;
			while (timer > 0f)
			{
				timer -= Time.get_deltaTime();
				int time = (int)timer + 1;
				if (time != lastSecond)
				{
					string time2 = $"{time / 60:D2}.{time % 60:D2}s";
					view.UpdateLabel(time2);
					view.PlaySound();
					lastSecond = time;
				}
				view.UpdateSprite(timer / (float)seconds);
				yield return null;
			}
			view.UpdateLabel("00.00s");
			view.UpdateSprite(0f);
		}

		private void ResetTimer(int id)
		{
			if (_task != null)
			{
				_task.Stop();
				view.UpdateLabel("00.00s");
				view.AnimateSprite(0f);
			}
		}
	}
}
