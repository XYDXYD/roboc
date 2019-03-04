using Battle;
using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Simulation.Sight;
using Simulation.SpawnEffects;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class BuildMachineClientCommandBase
	{
		[Inject]
		public BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
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
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager machineManager
		{
			protected get;
			set;
		}

		[Inject]
		public RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		public MachineClusterContainer machineClusterContainer
		{
			protected get;
			set;
		}

		[Inject]
		public WeaponRaycastContainer weaponRaycastContainer
		{
			private get;
			set;
		}

		[Inject]
		public MachineRootContainer machineRootcontainer
		{
			protected get;
			set;
		}

		[Inject]
		public MachineTimeManager machineTimeManager
		{
			protected get;
			set;
		}

		[Inject]
		public IMonoBehaviourFactory monoBehaviourFactory
		{
			protected get;
			set;
		}

		[Inject]
		public RemoteClientHistoryClient remoteClientHistory
		{
			protected get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			protected get;
			set;
		}

		[Inject]
		public MachineSpawnDispatcher machineSpawnDispatcher
		{
			protected get;
			set;
		}

		[Inject]
		public NetworkInputRecievedManager inputRecievedManager
		{
			private get;
			set;
		}

		[Inject]
		public ConnectedPlayersContainer connectedPlayers
		{
			private get;
			set;
		}

		[Inject]
		public RegisterPlayerObserver registerPlayerObserver
		{
			get;
			set;
		}

		[Inject]
		public MachineTeamColourUtility machineTeamColourUtility
		{
			private get;
			set;
		}

		[Inject]
		public IServerTimeClient serverTime
		{
			protected get;
			set;
		}

		[Inject]
		public IEntityFactory engineRoot
		{
			get;
			private set;
		}

		[Inject]
		public IGameObjectFactory gameobjectFactory
		{
			private get;
			set;
		}

		protected virtual void Build(int owner, int teamId, string name, bool isAi, string spawnEffect, string deathEffect)
		{
			Console.Log("building machine from " + owner);
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(name);
			preloadedMachine.machineBoard.set_name(preloadedMachine.machineBoard.get_name() + "_" + owner);
			BuildMachineEntity(owner, preloadedMachine, teamId, spawnEffect, deathEffect);
			RegisterMachineData(owner, teamId, preloadedMachine, name);
			bool flag = teamId == battlePlayers.MyTeam;
			preloadedMachine.machineBoard.SetActive(false);
			machineTeamColourUtility.SetRobotTeamColors(!flag, preloadedMachine.machineBoard);
			SetupMachineUpdaterForPlayer(owner, preloadedMachine);
			SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(owner, preloadedMachine.machineId, name, teamId, _isMe: false, flag, preloadedMachine, isAi, _isLocal: false);
			machineSpawnDispatcher.PlayerRegistered(spawnInParameters);
			registerPlayerObserver.RegisterPlayer(name, owner, isMe: false, flag);
			Console.Log("activated machine");
		}

		protected void SetupMachineUpdaterForPlayer(int player, PreloadedMachine preloadedMachine)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			remoteClientHistory.AddMachine(player, preloadedMachine.machineId, preloadedMachine.rbData);
			RemoteMachineTicker remoteMachineTicker = preloadedMachine.rbData.get_gameObject().AddComponent<RemoteMachineTicker>();
			MachineUpdaterClientPhysics machineUpdater = remoteMachineTicker.updater = new MachineUpdaterClientPhysics(player, remoteClientHistory.GetMachineHistory(preloadedMachine.machineId), preloadedMachine.rbData, preloadedMachine.weaponRaycast, commandFactory, preloadedMachine.machineInfo.MachineSize);
			machineTimeManager.RegisterUpdater(player, machineUpdater);
		}

		protected void RegisterMachineData(int player, int teamId, PreloadedMachine preloadedMachine, string name)
		{
			connectedPlayers.PlayerConnected(player, name);
		}

		private void BuildMachineEntity(int playerId, PreloadedMachine preloadedMachine, int teamId, string spawnEffect, string deathEffect)
		{
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			GameObject machineBoard = preloadedMachine.machineBoard;
			int machineId = preloadedMachine.machineId;
			Rigidbody rbData = preloadedMachine.rbData;
			playerMachinesContainer.RegisterPlayerMachine(TargetType.Player, playerId, machineId);
			machineClusterContainer.RegisterMachineCluster(TargetType.Player, machineId, preloadedMachine.machineGraph.cluster);
			machineClusterContainer.RegisterMicrobotCollisionSphere(TargetType.Player, machineId, preloadedMachine.machineGraph.sphere);
			machineRootcontainer.RegisterMachineRoot(TargetType.Player, machineId, machineBoard);
			rigidbodyDataContainer.RegisterRigidBodyData(TargetType.Player, machineId, rbData);
			machineManager.RegisterMachineMap(TargetType.Player, machineId, preloadedMachine.machineMap);
			weaponRaycastContainer.RegisterWeaponRaycast(machineId, preloadedMachine.weaponRaycast);
			inputRecievedManager.RegisterUsersInputWrapper(playerId, preloadedMachine.inputWrapper as MachineInputWrapperRemoteClient);
			bool flag = teamId == battlePlayers.MyTeam;
			MachineOwnerImplementor machineOwnerImplementor = new MachineOwnerImplementor();
			machineOwnerImplementor.SetOwnedByMe(ownedByMe_: false);
			machineOwnerImplementor.SetOwnedByAi(ownedByAi_: false);
			machineOwnerImplementor.SetOwner(playerId, machineId);
			machineOwnerImplementor.ownerTeamId = teamId;
			MachineInvisibilityImplementor machineInvisibilityImplementor = new MachineInvisibilityImplementor();
			AudioGameObjectComponentImplementor audioGameObjectComponentImplementor = new AudioGameObjectComponentImplementor(preloadedMachine.machineInfo.centerTransform.get_gameObject());
			MachineRigidbodyTransformImplementor machineRigidbodyTransformImplementor = new MachineRigidbodyTransformImplementor(rbData);
			MachineStunImplementor machineStunImplementor = new MachineStunImplementor();
			SpottableImplementor spottableImplementor = new SpottableImplementor(machineId);
			AliveStateImplementor aliveStateImplementor = new AliveStateImplementor(machineId);
			MachineInputImplementor machineInputImplementor = new MachineInputImplementor(preloadedMachine.inputWrapper);
			MachineRaycastImplementor machineRaycastImplementor = new MachineRaycastImplementor(preloadedMachine.weaponRaycast);
			WeaponOrderImplementor weaponOrderImplementor = new WeaponOrderImplementor(preloadedMachine.weaponOrder);
			MachineTargetsImplementor machineTargetsImplementor = new MachineTargetsImplementor();
			GameObject val = gameobjectFactory.Build(spawnEffect);
			SpawnEffectImplementor component = val.GetComponent<SpawnEffectImplementor>();
			val.SetActive(false);
			SpawnEffectDependenciesImplementor spawnEffectDependenciesImplementor = new SpawnEffectDependenciesImplementor(rbData, preloadedMachine.machineInfo.MachineCenter, preloadedMachine.machineInfo.MachineSize, preloadedMachine.allRenderers);
			val = gameobjectFactory.Build(deathEffect);
			DeathEffectImplementor component2 = val.GetComponent<DeathEffectImplementor>();
			val.SetActive(false);
			DeathEffectDependenciesImplementor deathEffectDependenciesImplementor = new DeathEffectDependenciesImplementor(rbData, rbData.get_transform().get_parent().get_gameObject(), preloadedMachine.machineInfo.MachineCenter, preloadedMachine.machineInfo.MachineSize);
			MonoBehaviour[] components = machineBoard.GetComponents<MonoBehaviour>();
			FasterList<object> val2 = new FasterList<object>((ICollection<object>)components);
			val2.Add((object)machineOwnerImplementor);
			val2.Add((object)audioGameObjectComponentImplementor);
			val2.Add((object)machineRigidbodyTransformImplementor);
			val2.Add((object)spottableImplementor);
			val2.Add((object)aliveStateImplementor);
			val2.Add((object)machineInvisibilityImplementor);
			val2.Add((object)machineStunImplementor);
			val2.Add((object)machineInputImplementor);
			val2.Add((object)machineRaycastImplementor);
			val2.Add((object)weaponOrderImplementor);
			val2.Add((object)new MachineTopSpeedImplementor());
			val2.Add((object)new MachineHealingPriorityImplementor());
			val2.Add((object)new MachineFunctionalImplementor(machineId));
			val2.Add((object)machineTargetsImplementor);
			val2.Add((object)new InputMotorComponent());
			val2.Add((object)component);
			val2.Add((object)spawnEffectDependenciesImplementor);
			val2.Add((object)component2);
			val2.Add((object)deathEffectDependenciesImplementor);
			val2.Add((object)new EntitySourceComponent(isLocal: false));
			FasterList<IEntityViewBuilder> val3 = new FasterList<IEntityViewBuilder>();
			RegistrationHelper.CheckRemoteCubesNodes(preloadedMachine, val2, val3);
			if (val3.get_Count() != 0)
			{
				engineRoot.BuildEntity(machineId, new DynamicEntityDescriptorInfo<RemoteMachineEntityDescriptor>(val3), val2.ToArray());
			}
			else
			{
				engineRoot.BuildEntity<RemoteMachineEntityDescriptor>(machineId, val2.ToArray());
			}
		}
	}
}
