using Authentication;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal abstract class SinglePlayerRespawner : IInitialize, IWaitForFrameworkDestruction
	{
		private ITaskRoutine _task;

		[Inject]
		internal MachineSpawnDispatcher dispatcher
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
		internal PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer _playerNames
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			dispatcher.OnPlayerRespawnScheduled += HandleOnRespawnScheduled;
			BuildRespawnCommand();
		}

		protected abstract void BuildRespawnCommand();

		protected abstract void ExecuteRespawnCommand(PlayerIdDependency dependency);

		public void OnFrameworkDestroyed()
		{
			dispatcher.OnPlayerRespawnScheduled -= HandleOnRespawnScheduled;
			if (_task != null)
			{
				_task.Stop();
			}
		}

		public virtual void GetRespawnPoint()
		{
			string username = User.Username;
			int playerId = _playerNames.GetPlayerId(username);
			PlayerIdDependency dependency = new PlayerIdDependency(playerId);
			ExecuteRespawnCommand(dependency);
		}

		private void HandleOnRespawnScheduled(int playerId, int seconds)
		{
			if (playerTeams.IsMe(TargetType.Player, playerId))
			{
				_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)(() => WaitTimer(playerId, seconds)));
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator WaitTimer(int playerId, int time)
		{
			float timer = 0f;
			while (timer < (float)time)
			{
				timer += Time.get_deltaTime();
				yield return null;
			}
			dispatcher.LocalPlayerReadyToRespawn(playerId);
		}
	}
}
