using Simulation.Hardware.Weapons;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal sealed class DestructionSyncReplayer
	{
		private Queue<SyncMachineCubesDependency> _pending = new Queue<SyncMachineCubesDependency>();

		private ITaskRoutine _process;

		[Inject]
		internal NetworkMachineManager _machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootUpdater _rootUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer _playerMachines
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionManager _destructionManager
		{
			private get;
			set;
		}

		[Inject]
		internal HealingManager _healingManager
		{
			private get;
			set;
		}

		public bool isFinished => _process == null;

		public void Replay(SyncMachineCubesDependency dep)
		{
			Console.Log("Queue replay " + dep.history.Count + " events on machine " + dep.machineId);
			_pending.Enqueue(dep);
			if (_process == null)
			{
				_process = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Process);
				_process.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private static HitCubeInfo ToHitCubeInfo(CubeHistoryEvent e, int machineId)
		{
			HitCubeInfo result = default(HitCubeInfo);
			result.damage = e.damage;
			result.gridLoc = e.gridLoc;
			result.destroyed = (e.type == CubeHistoryEvent.Type.Destroy);
			return result;
		}

		private IEnumerator Process()
		{
			List<HitCubeInfo> batch = new List<HitCubeInfo>();
			int batchMax = 100;
			while (_pending.Count != 0)
			{
				SyncMachineCubesDependency dep = _pending.Dequeue();
				IMachineMap machineMap = _machineManager.GetMachineMap(TargetType.Player, dep.machineId);
				int targetPlayerId = _playerMachines.GetPlayerFromMachineId(TargetType.Player, dep.machineId);
				List<CubeHistoryEvent> history = dep.history;
				batch.Add(ToHitCubeInfo(history[0], dep.machineId));
				for (int i = 1; i < history.Count; i++)
				{
					CubeHistoryEvent cubeHistoryEvent = history[i];
					bool isHeal = cubeHistoryEvent.type == CubeHistoryEvent.Type.Heal;
					CubeHistoryEvent cubeHistoryEvent2 = history[i - 1];
					bool prevIsHeal = cubeHistoryEvent2.type == CubeHistoryEvent.Type.Heal;
					if (batch.Count >= batchMax || (isHeal ^ prevIsHeal))
					{
						ProcessBatch(batch, machineMap, dep.machineId, targetPlayerId, prevIsHeal);
						batch.Clear();
						yield return null;
					}
					batch.Add(ToHitCubeInfo(history[i], dep.machineId));
				}
				if (batch.Count != 0)
				{
					CubeHistoryEvent cubeHistoryEvent3 = history[history.Count - 1];
					ProcessBatch(isHeal: cubeHistoryEvent3.type == CubeHistoryEvent.Type.Heal, history: batch, machineMap: machineMap, targetMachineId: dep.machineId, targetPlayerId: targetPlayerId);
					batch.Clear();
					yield return null;
				}
				Console.Log("Finished replaying for machine " + dep.machineId);
			}
			_process = null;
		}

		private void ProcessBatch(List<HitCubeInfo> history, IMachineMap machineMap, int targetMachineId, int targetPlayerId, bool isHeal)
		{
			if (isHeal)
			{
				_healingManager.PerformHealing(history, -1, targetMachineId, TargetType.Player, TargetType.Player, playEffects: false, isReconnecting: true);
			}
			else
			{
				_destructionManager.PerformDestruction(history, -1, targetMachineId, targetPlayerId, TargetType.Player, targetIsMe: false, playEffects: true, 0, isReconnecting: true);
			}
		}
	}
}
