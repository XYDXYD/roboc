using RCNetwork.Events;
using Simulation.Hardware.Modules.Emp.Observers;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleStunEngine : SingleEntityViewEngine<EmpLocatorStunNode>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private bool _isSinglePlayerMatch;

		private NetworkStunnedMachineEffectDependency _dependency = new NetworkStunnedMachineEffectDependency();

		private NetworkStunMachineObserver _networkStunMachineObserver;

		private MachineStunnedData _machineStunnedData = new MachineStunnedData(0, isStunned_: false);

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal InputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachineStunnedObservable machineStunnedObservable
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe EmpModuleStunEngine(bool isSinglePlayer, NetworkStunMachineObserver observer)
		{
			_isSinglePlayerMatch = isSinglePlayer;
			_networkStunMachineObserver = observer;
			_networkStunMachineObserver.AddAction(new ObserverAction<NetworkStunnedMachineData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_networkStunMachineObserver.RemoveAction(new ObserverAction<NetworkStunnedMachineData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(EmpLocatorStunNode stunNode)
		{
			stunNode.activationComponent.activateEmpStun.subscribers += ActivateStun;
		}

		protected override void Remove(EmpLocatorStunNode stunNode)
		{
			stunNode.activationComponent.activateEmpStun.subscribers -= ActivateStun;
		}

		private void ActivateStun(IEmpStunActivationComponent sender, int locatorId)
		{
			EmpLocatorStunNode empLocatorStunNode = default(EmpLocatorStunNode);
			if (!entityViewsDB.TryQueryEntityView<EmpLocatorStunNode>(locatorId, ref empLocatorStunNode))
			{
				return;
			}
			if (!_isSinglePlayerMatch)
			{
				if (!empLocatorStunNode.ownerComponent.isOnMyTeam)
				{
					int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
					TryStunMachine(empLocatorStunNode, locatorId, activeMachine);
				}
				return;
			}
			List<int> playersOnEnemyTeam = playerTeamsContainer.GetPlayersOnEnemyTeam(TargetType.Player);
			List<int>.Enumerator enumerator = playersOnEnemyTeam.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int activeMachine2 = playerMachinesContainer.GetActiveMachine(TargetType.Player, enumerator.Current);
				TryStunMachine(empLocatorStunNode, locatorId, activeMachine2);
			}
		}

		private void TryStunMachine(EmpLocatorStunNode empStunNode, int locatorId, int machineId)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			MachineStunNode machineStunNode = default(MachineStunNode);
			if (!entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode))
			{
				return;
			}
			Vector3 val = machineStunNode.rigidbodyComponent.rb.get_worldCenterOfMass() - empStunNode.transformComponent.empLocatorTransform.get_position();
			val.y = 0f;
			float range = empStunNode.rangeComponent.range;
			if (Vector3.SqrMagnitude(val) <= range * range)
			{
				machineStunNode.stunComponent.stunned = true;
				machineStunNode.stunComponent.stunningEmpLocator = locatorId;
				machineStunNode.stunComponent.stunTimer = empStunNode.stunDurationComponent.stunDuration;
				machineStunNode.stunComponent.machineStunned.Dispatch(ref machineId);
				if (!machineStunNode.ownerComponent.ownedByAi)
				{
					_dependency.SetValues(machineId, isStunned_: true, machineStunNode.ownerComponent.ownerId);
					networkManager.SendEventToServer(NetworkEvent.BroadcastSpawnEmpMachineEffect, _dependency);
					_machineStunnedData.SetValues(machineId, isStunned_: true);
					machineStunnedObservable.Dispatch(ref _machineStunnedData);
				}
			}
		}

		private void HandleRemoteMachineStunned(ref NetworkStunnedMachineData data)
		{
			MachineStunNode machineStunNode = default(MachineStunNode);
			if (entityViewsDB.TryQueryEntityView<MachineStunNode>(data.machineId, ref machineStunNode))
			{
				machineStunNode.stunComponent.stunned = data.isStunned;
				machineStunNode.stunComponent.remoteMachineStunned.Dispatch(ref data.machineId);
			}
		}
	}
}
