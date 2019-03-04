using Simulation.Hardware.Modules.Emp.Observers;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class MechLegController : IWaitForFrameworkInitialization
	{
		protected Dictionary<int, MechLegManager> _legManagers = new Dictionary<int, MechLegManager>();

		private MachineStunnedObserver _machineStunnedObsever;

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
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
		public PlayerStrafeDirectionManager playerStrafeDirectionManager
		{
			protected get;
			set;
		}

		internal void Unregister(int machineId)
		{
			_legManagers[machineId].Unregister();
			_legManagers.Remove(machineId);
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_updateScheduler(), (Func<IEnumerator>)Tick);
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_physicScheduler(), (Func<IEnumerator>)PhysicsTick);
		}

		internal void Register(bool ownedByMe, bool ownedByAi, int machineId, Rigidbody rb, IMachineControl machineInput)
		{
			MechLegManager mechLegManager = ownedByMe ? new LocalMechLegManager(rb, machineManager, machineInput, playerStrafeDirectionManager) : ((!ownedByAi) ? ((MechLegManager)new RemoteMechLegManager(rb, machineManager, machineInput)) : ((MechLegManager)new LocalMechLegManager(rb, machineManager, machineInput)));
			mechLegManager.SetMachineId(machineId);
			_legManagers[machineId] = mechLegManager;
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				Dictionary<int, MechLegManager>.Enumerator enumerator = _legManagers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.Tick(Time.get_deltaTime());
				}
				yield return null;
			}
		}

		private IEnumerator PhysicsTick()
		{
			while (true)
			{
				Dictionary<int, MechLegManager>.Enumerator enumerator = _legManagers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.PhysicsTick(Time.get_fixedDeltaTime());
				}
				yield return null;
			}
		}

		public void SetPauseStateForLocalMechLegs(bool value)
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
			if (_legManagers.TryGetValue(activeMachine, out MechLegManager value2))
			{
				value2.PauseLegs(value);
			}
		}

		public void SetJumpStateForLocalMechLegs()
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
			if (_legManagers.TryGetValue(activeMachine, out MechLegManager value))
			{
				value.SetJumpState();
			}
		}

		internal void SetMachineLegsEnabled(int machineId, bool enabled)
		{
			MechLegManager mechLegManager = _legManagers[machineId];
			if (enabled)
			{
				mechLegManager.EnableMechLegs();
			}
			else
			{
				mechLegManager.DisableMechLegs();
			}
		}

		public void RegisterLeg(CubeMechLeg leg, int machineId)
		{
			_legManagers[machineId].RegisterLeg(leg);
		}

		public void UnregisterLeg(CubeMechLeg leg, int machineId)
		{
			_legManagers[machineId].UnregisterLeg(leg);
		}
	}
}
