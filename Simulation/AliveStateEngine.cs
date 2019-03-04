using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal class AliveStateEngine : SingleEntityViewEngine<MachineAliveStateNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<int, MachineAliveStateNode> _nodesByPlayerId = new Dictionary<int, MachineAliveStateNode>();

		[Inject]
		internal MachineSpawnDispatcher machineDispatcher
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

		public void OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
			machineDispatcher.OnPlayerSpawnedIn += OnMachineSpawned;
			machineDispatcher.OnPlayerRespawnedIn += OnMachineSpawned;
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= OnMachineDestroyed;
			machineDispatcher.OnPlayerSpawnedIn -= OnMachineSpawned;
			machineDispatcher.OnPlayerRespawnedIn -= OnMachineSpawned;
		}

		private void OnMachineSpawned(SpawnInParametersPlayer parameters)
		{
			SetAliveState(parameters.playerId, alive: true);
		}

		private void OnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			SetAliveState(playerId, alive: false);
		}

		private void SetAliveState(int playerId, bool alive)
		{
			if (_nodesByPlayerId.TryGetValue(playerId, out MachineAliveStateNode value))
			{
				value.aliveStateComponent.isAlive.set_value(alive);
			}
		}

		protected override void Add(MachineAliveStateNode node)
		{
			_nodesByPlayerId.Add(node.ownerComponent.ownerId, node);
		}

		protected override void Remove(MachineAliveStateNode node)
		{
			_nodesByPlayerId.Remove(node.ownerComponent.ownerId);
		}
	}
}
