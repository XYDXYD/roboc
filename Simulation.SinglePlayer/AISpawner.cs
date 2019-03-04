using LobbyServiceLayer;
using RCNetwork.Events;
using RCNetwork.Server;
using Simulation.SinglePlayer.Spawn;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal class AISpawner : MultiEntityViewsEngine<AIAgentDataComponentsNode, PlayerTargetNode>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private class RespawnQueueData
		{
			public int botId
			{
				get;
				private set;
			}

			public int botTeamId
			{
				get;
				private set;
			}

			public float respawnTime
			{
				get;
				private set;
			}

			public RespawnQueueData(int id, int teamId, float respawnTime)
			{
				botId = id;
				botTeamId = teamId;
				this.respawnTime = respawnTime;
			}
		}

		private const float FREE_SPAWN_POINT_CHECK_DISTANCE = 7f;

		private const float RESPAWN_TIME = 3f;

		public const float MAX_SPEED = 25f;

		public const float MAX_TURNING_SPEED = 90f;

		private PlayerTargetNode _playerTargetNode;

		private FasterList<AIAgentDataComponentsNode> _agents = new FasterList<AIAgentDataComponentsNode>();

		private float _time;

		private LinkedList<RespawnQueueData> _respawnQueue = new LinkedList<RespawnQueueData>();

		private RespawnAtPositionClientCommand _respawnCommand;

		private AllPlayersSpawnedObservable _allPlayersSpawnedObservable;

		private float _maxPlayerMachineRadius;

		private bool _gameEnded;

		[Inject]
		public ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public ISpawnPointManager spawnPointManager
		{
			private get;
			set;
		}

		[Inject]
		public MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		public ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		public GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public AISpawner(AllPlayersSpawnedObservable observable)
		{
			_allPlayersSpawnedObservable = observable;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_respawnCommand = commandFactory.Build<RespawnAtPositionClientCommand>();
			destructionReporter.OnMachineKilled += HandleOnMachineKilled;
			gameEndedObserver.OnGameEnded += HandleGameEnded;
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_physicScheduler(), (Func<IEnumerator>)PhysicsTick);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= HandleOnMachineKilled;
			gameEndedObserver.OnGameEnded -= HandleGameEnded;
		}

		private IEnumerator PhysicsTick()
		{
			while (!_gameEnded)
			{
				_time += Time.get_fixedDeltaTime();
				bool checkRespawn = true;
				LinkedListNode<RespawnQueueData> iter = _respawnQueue.First;
				while (checkRespawn && iter != null)
				{
					LinkedListNode<RespawnQueueData> next = iter.Next;
					if (_time >= iter.Value.respawnTime)
					{
						SpawningPoint spawningPoint = FindFreeSpawnPoint(iter.Value.botTeamId, iter.Value.botId);
						SpawnPointDependency dependency = new SpawnPointDependency(spawningPoint.get_transform().get_position(), spawningPoint.get_transform().get_rotation(), iter.Value.botId);
						_respawnCommand.Inject(dependency);
						_respawnCommand.Execute();
						_respawnQueue.Remove(iter);
					}
					else
					{
						checkRespawn = false;
					}
					iter = next;
				}
				yield return null;
			}
		}

		protected override void Add(PlayerTargetNode obj)
		{
			_playerTargetNode = obj;
			RetrieveExpectedPlayersAndSpawnAllBots();
		}

		protected override void Remove(PlayerTargetNode obj)
		{
			if (obj != null)
			{
				_playerTargetNode = null;
			}
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			float horizontalRadius = obj.aiMovementData.horizontalRadius;
			_agents.Add(obj);
			_maxPlayerMachineRadius = Math.Max(horizontalRadius, _maxPlayerMachineRadius);
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_agents.UnorderedRemove(obj);
		}

		private SpawningPoint FindFreeSpawnPoint(int teamId, int playerId)
		{
			SpawnPoints.SpawnPointsType spawnPointType = (teamId != 0) ? SpawnPoints.SpawnPointsType.Team1Start : SpawnPoints.SpawnPointsType.Team0Start;
			SpawningPoint nextFreeSpawnPoint = spawnPointManager.GetNextFreeSpawnPoint(spawnPointType, playerId);
			if (IsSpawnPointFree(nextFreeSpawnPoint))
			{
				return nextFreeSpawnPoint;
			}
			SpawningPoint nextFreeSpawnPoint2 = spawnPointManager.GetNextFreeSpawnPoint(spawnPointType, playerId);
			bool flag = false;
			while (!flag && nextFreeSpawnPoint2.GetInstanceID() != nextFreeSpawnPoint.GetInstanceID())
			{
				if (IsSpawnPointFree(nextFreeSpawnPoint2))
				{
					flag = true;
				}
				else
				{
					nextFreeSpawnPoint2 = spawnPointManager.GetNextFreeSpawnPoint(spawnPointType, playerId);
				}
			}
			return nextFreeSpawnPoint2;
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
			for (int i = 0; i < _agents.get_Count(); i++)
			{
				Vector3 position2 = _agents.get_Item(i).aiMovementData.position;
				if (Vector3.SqrMagnitude(position2 - position) < 49f)
				{
					return false;
				}
			}
			Vector3 worldCenterOfMass = _playerTargetNode.playerTargetGameObjectComponent.rigidBody.get_worldCenterOfMass();
			return Vector3.SqrMagnitude(worldCenterOfMass - position) >= 49f;
		}

		private void RetrieveExpectedPlayersAndSpawnAllBots()
		{
			lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(SpawnAllBots)).Execute();
		}

		private void SpawnAllBots(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			DictionaryEnumerator<string, PlayerDataDependency> enumerator = expectedPlayers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerDataDependency value = enumerator.get_Current().Value;
					if (value.AiPlayer)
					{
						SpawnPoints.SpawnPointsType spawnPointType = (value.TeamId != 0) ? SpawnPoints.SpawnPointsType.Team1Start : SpawnPoints.SpawnPointsType.Team0Start;
						int playerId = playerNamesContainer.GetPlayerId(value.PlayerName);
						SpawningPoint nextFreeSpawnPoint = spawnPointManager.GetNextFreeSpawnPoint(spawnPointType, playerId);
						PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(value.PlayerName);
						Vector3 worldCenterOfMass = preloadedMachine.rbData.get_worldCenterOfMass();
						MachineDefinitionDependency machineDefinitionDependency = new MachineDefinitionDependency(playerId, value.TeamId, nextFreeSpawnPoint.get_transform().get_position(), nextFreeSpawnPoint.get_transform().get_rotation(), 0, worldCenterOfMass);
						RegisterAIMachineCommand registerAIMachineCommand = commandFactory.Build<RegisterAIMachineCommand>();
						registerAIMachineCommand.Initialise(machineDefinitionDependency.owner, machineDefinitionDependency.teamId, value.PlayerName, preloadedMachine, 25f, 90f, value.TeamId == _playerTargetNode.playerTargetGameObjectComponent.teamId, "Spawn", "Explosion");
						registerAIMachineCommand.Execute();
						networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeSpawnPoint, playerId, new SpawnPointDependency(nextFreeSpawnPoint.get_transform().get_position(), nextFreeSpawnPoint.get_transform().get_rotation(), playerId));
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			_allPlayersSpawnedObservable.Dispatch();
		}

		private void HandleOnMachineKilled(int ownerId, int shooterId)
		{
			int num = 0;
			while (true)
			{
				if (num < _agents.get_Count())
				{
					if (_agents.get_Item(num).aiBotIdData.playerId == ownerId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			ScheduleRespawnOnKill(_agents.get_Item(num).aiBotIdData);
		}

		private void ScheduleRespawnOnKill(IAIBotIdDataComponent botIdData)
		{
			RespawnQueueData value = new RespawnQueueData(botIdData.playerId, botIdData.teamId, _time + 3f);
			_respawnQueue.AddLast(value);
		}

		private void HandleGameEnded(bool victory)
		{
			_gameEnded = true;
		}

		public void Ready()
		{
		}
	}
}
