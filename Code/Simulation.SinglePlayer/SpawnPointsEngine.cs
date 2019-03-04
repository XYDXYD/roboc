using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal sealed class SpawnPointsEngine : MultiEntityViewsEngine<AIAgentDataComponentsNode, PlayerTargetNode, SpawnPointsNode>, IWaitForFrameworkDestruction
	{
		private PlayerTargetNode _playerTargetNode;

		private FasterList<AIAgentDataComponentsNode> _behaviorTrees = new FasterList<AIAgentDataComponentsNode>();

		private FasterList<SpawnPointsNode> _spawnPointsNodes = new FasterList<SpawnPointsNode>();

		private PlayerSpawnPointObserver _playerSpawnPointObserver;

		private Random _rand = new Random(DateTime.UtcNow.Millisecond);

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			get;
			set;
		}

		public SpawnPointsEngine(PlayerSpawnPointObserver observer)
		{
			_playerSpawnPointObserver = observer;
			_playerSpawnPointObserver.AddAction((Action)HandleHumanPlayerSpawnPointRequest);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_playerSpawnPointObserver.RemoveAction((Action)HandleHumanPlayerSpawnPointRequest);
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			_behaviorTrees.Add(obj);
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_behaviorTrees.UnorderedRemove(obj);
		}

		protected override void Add(SpawnPointsNode obj)
		{
			_spawnPointsNodes.Add(obj);
		}

		protected override void Remove(SpawnPointsNode obj)
		{
			_spawnPointsNodes.UnorderedRemove(obj);
		}

		protected override void Add(PlayerTargetNode obj)
		{
			_playerTargetNode = obj;
		}

		protected override void Remove(PlayerTargetNode obj)
		{
			_playerTargetNode = null;
		}

		private void HandleHumanPlayerSpawnPointRequest()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			SpawnPoints.SpawnPointsType spawnPointsType = SpawnPoints.SpawnPointsType.Team0Start;
			int playerId = _playerTargetNode.playerTargetGameObjectComponent.playerId;
			spawnPointsType = ((_playerTargetNode.playerTargetGameObjectComponent.teamId != 0) ? SpawnPoints.SpawnPointsType.Team1Start : SpawnPoints.SpawnPointsType.Team0Start);
			ISpawnPointsComponent spawnPoints = FindGroupSpawningPoints(spawnPointsType);
			SpawningPoint spawningPoint = FindFreeSpawnPoint(spawnPoints);
			SpawnPointDependency dependency = new SpawnPointDependency(spawningPoint.get_transform().get_position(), spawningPoint.get_transform().get_rotation(), playerId);
			networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeRespawnPoint, playerId, dependency);
		}

		private ISpawnPointsComponent FindGroupSpawningPoints(SpawnPoints.SpawnPointsType groupType)
		{
			for (int i = 0; i < _spawnPointsNodes.get_Count(); i++)
			{
				ISpawnPointsComponent spawnPoints = _spawnPointsNodes.get_Item(i).spawnPoints;
				if (spawnPoints.spawningPointsType == groupType)
				{
					return spawnPoints;
				}
			}
			return null;
		}

		private SpawningPoint FindFreeSpawnPoint(ISpawnPointsComponent spawnPoints)
		{
			int num = _rand.Next(0, spawnPoints.spawningPointList.Length);
			for (int i = 0; i < spawnPoints.spawningPointList.Length; i++)
			{
				num = (num + i) % spawnPoints.spawningPointList.Length;
				SpawningPoint spawningPoint = spawnPoints.spawningPointList[num];
				if (IsSpawnPointFree(spawningPoint))
				{
					return spawningPoint;
				}
			}
			return spawnPoints.spawningPointList[0];
		}

		private bool IsSpawnPointFree(SpawningPoint spawningPoint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = spawningPoint.get_transform().get_position();
			for (int i = 0; i < _behaviorTrees.get_Count(); i++)
			{
				Vector3 position2 = _behaviorTrees.get_Item(i).aiMovementData.position;
				if (Vector3.SqrMagnitude(position2 - position) < 50f)
				{
					return false;
				}
			}
			Vector3 worldCenterOfMass = _playerTargetNode.playerTargetGameObjectComponent.rigidBody.get_worldCenterOfMass();
			return Vector3.SqrMagnitude(worldCenterOfMass - position) >= 50f;
		}
	}
}
