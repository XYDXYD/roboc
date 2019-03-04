using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class DamageBoostEngine : SingleEntityViewEngine<DamageMultiplierNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private DamageBoostDeserialisedData _damageBoostDataTable;

		private Dictionary<int, uint> _playerCPUsMap = new Dictionary<int, uint>();

		private FasterList<DamageMultiplierNode> _unprocessedNodes = new FasterList<DamageMultiplierNode>();

		private FasterList<DamageMultiplierNode> _processedNodes = new FasterList<DamageMultiplierNode>();

		[Inject]
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal MachineCpuDataManager cpuDataManager
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(OnGameStart);
			cpuDataManager.OnMachineCpuInitialized += OnMachineCpuInitialized;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
			cpuDataManager.OnMachineCpuInitialized -= OnMachineCpuInitialized;
		}

		private void OnMachineCpuInitialized(int playerId, uint initialCpu)
		{
			_playerCPUsMap[playerId] = initialCpu;
			ReconcileData();
		}

		private void OnGameStart()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndApplyDamageBoost);
		}

		protected override void Add(DamageMultiplierNode node)
		{
			if (!_unprocessedNodes.Contains(node) && !_processedNodes.Contains(node))
			{
				_unprocessedNodes.Add(node);
				ReconcileData();
			}
		}

		protected override void Remove(DamageMultiplierNode node)
		{
			if (_unprocessedNodes.Contains(node))
			{
				_unprocessedNodes.Remove(node);
			}
		}

		private IEnumerator LoadAndApplyDamageBoost()
		{
			IGetDamageBoostRequest request = serviceRequestFactory.Create<IGetDamageBoostRequest>();
			TaskService<DamageBoostDeserialisedData> task = new TaskService<DamageBoostDeserialisedData>(request);
			yield return task;
			if (task.succeeded)
			{
				_damageBoostDataTable = (DamageBoostDeserialisedData)task.result.Clone();
				ReconcileData();
			}
			else
			{
				RemoteLogger.Error("Error processing damage boost table", " failed to retrieve damage boost data", null);
			}
		}

		private void ReconcileData()
		{
			if (_damageBoostDataTable == null)
			{
				return;
			}
			for (int i = 0; i < _unprocessedNodes.get_Count(); i++)
			{
				DamageMultiplierNode damageMultiplierNode = _unprocessedNodes.get_Item(i);
				int ownerId = damageMultiplierNode.ownerComponent.ownerId;
				if (_playerCPUsMap.ContainsKey(ownerId))
				{
					uint cpu = _playerCPUsMap[ownerId];
					float damageBoost = _damageBoostDataTable.CalculateNearestBoost(cpu);
					damageMultiplierNode.damageStats.damageBoost = damageBoost;
					_processedNodes.Add(damageMultiplierNode);
				}
			}
			for (int j = 0; j < _processedNodes.get_Count(); j++)
			{
				DamageMultiplierNode damageMultiplierNode2 = _processedNodes.get_Item(j);
				_unprocessedNodes.Remove(damageMultiplierNode2);
			}
		}
	}
}
