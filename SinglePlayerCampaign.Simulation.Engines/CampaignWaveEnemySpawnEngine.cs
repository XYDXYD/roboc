using RCNetwork.Events;
using RCNetwork.Server;
using Simulation;
using Simulation.GUI;
using Simulation.SinglePlayer;
using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.EntityViews;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveEnemySpawnEngine : SingleEntityViewEngine<CampaignWaveEnemySpawnEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const int TEAM_ID = 1;

		private const float MAX_SPEED = 25f;

		private const float MAX_TURNING_SPEED = 90f;

		private readonly MachinePreloader _machinePreloader;

		private readonly PlayerNamesContainer _playerNamesContainer;

		private readonly ISpawnPointManager _spawnPointManager;

		private readonly ICommandFactory _commandFactory;

		private readonly ICubeList _cubeList;

		private readonly INetworkEventManagerServer _networkEventManagerServer;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignWaveEnemySpawnEngine(MachinePreloader machinePreloader, PlayerNamesContainer playerNamesContainer, ISpawnPointManager spawnPointManager, ICommandFactory commandFactory, ICubeList cubeList, INetworkEventManagerServer networkEventManagerServer)
		{
			_machinePreloader = machinePreloader;
			_playerNamesContainer = playerNamesContainer;
			_spawnPointManager = spawnPointManager;
			_commandFactory = commandFactory;
			_cubeList = cubeList;
			_networkEventManagerServer = networkEventManagerServer;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveEnemySpawnEntityView entityView)
		{
			entityView.spawnRequestComponent.spawnRequests.NotifyOnValueSet((Action<int, IEnumerable<SpawnRequest>>)SpawnRobots);
		}

		protected override void Remove(CampaignWaveEnemySpawnEntityView entityView)
		{
			entityView.spawnRequestComponent.spawnRequests.StopNotify((Action<int, IEnumerable<SpawnRequest>>)SpawnRobots);
		}

		private void SpawnRobots(int entityId, IEnumerable<SpawnRequest> spawnRequests)
		{
			foreach (SpawnRequest spawnRequest in spawnRequests)
			{
				TaskRunner.get_Instance().Run(SpawnRobot(spawnRequest));
			}
		}

		private IEnumerator SpawnRobot(SpawnRequest spawnRequest)
		{
			PreloadedMachine preloadedMachine = _machinePreloader.GetPreloadedMachine(spawnRequest.robotName);
			preloadedMachine.rbData.set_isKinematic(true);
			int playerId = _playerNamesContainer.GetPlayerId(spawnRequest.robotName);
			SpawningPoint spawningPoint = _spawnPointManager.GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType.PitModeStartLocations, playerId);
			RegisterAIMachineCommand registerAiMachineCommand = _commandFactory.Build<RegisterAIMachineCommand>();
			registerAiMachineCommand.Initialise(playerId, 1, spawnRequest.robotName, preloadedMachine, 25f, 90f, humanPlayerTeam: false, "Spawn", "Explosion", spawnRequest.spawnEventId, spawnRequest.isKillRequirement);
			registerAiMachineCommand.Execute();
			int firstItemKey = preloadedMachine.weaponOrder.GetFirstItemDescriptorKey();
			if (firstItemKey != 0)
			{
				ItemDescriptor itemDescriptorFromCube = _cubeList.GetItemDescriptorFromCube(firstItemKey);
				SetSelectedWeaponClientCommand setSelectedWeaponClientCommand = _commandFactory.Build<SetSelectedWeaponClientCommand>();
				SelectWeaponDependency selectWeaponDependency = new SelectWeaponDependency();
				selectWeaponDependency.SetParameters(preloadedMachine.machineId, itemDescriptorFromCube);
				setSelectedWeaponClientCommand.Inject(selectWeaponDependency);
				setSelectedWeaponClientCommand.Execute();
			}
			else
			{
				Console.LogWarning("Unable to find weapons in Bot machine " + spawnRequest.robotName);
			}
			if (spawnRequest.isBoss)
			{
				FasterListEnumerator<HealthBarShowEntityView> enumerator = entityViewsDB.QueryEntityViews<HealthBarShowEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						HealthBarShowEntityView current = enumerator.get_Current();
						current.healthBarMachineIdComponent.machineId = preloadedMachine.machineId;
						current.healthBarMachineIdComponent.isActive.set_value(true);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			_networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeSpawnPoint, playerId, new SpawnPointDependency(spawningPoint.get_transform().get_position(), spawningPoint.get_transform().get_rotation(), playerId));
			yield return null;
			preloadedMachine.rbData.set_isKinematic(false);
		}

		private PathNode GetRandomWaypoint(int teamId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AIWaypointListNode> val = entityViewsDB.QueryEntityViews<AIWaypointListNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				AIWaypointListNode aIWaypointListNode = val.get_Item(i);
				if (teamId == aIWaypointListNode.aiWaypointListComponent.teamId)
				{
					Random random = new Random(DateTime.UtcNow.Millisecond);
					PathNode[] nodes = aIWaypointListNode.aiWaypointListComponent.nodes;
					int num = random.Next(0, nodes.Length);
					return nodes[num];
				}
			}
			return null;
		}
	}
}
