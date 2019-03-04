using Simulation.Hardware.Weapons;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class LegController : ITickable, IPhysicallyTickable, ITickableBase
	{
		private Dictionary<int, LegManager> _legManagers = new Dictionary<int, LegManager>();

		[Inject]
		public MachineSpawnDispatcher spawnDispatcher
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
		public MachineRootContainer machineRootContainer
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
		public PlayerStrafeDirectionManager playerStrafeDirectionManager
		{
			private get;
			set;
		}

		internal void Unregister(int machineId)
		{
			_legManagers[machineId].Unregister();
			_legManagers.Remove(machineId);
		}

		internal void Register(bool ownedByMe, bool ownedByAi, int machineId, Rigidbody rb, IMachineControl machineInput)
		{
			LegManager value = ownedByMe ? new LocalLegManager(rb, machineInput, playerStrafeDirectionManager) : ((!ownedByAi) ? ((LegManager)new RemoteLegManager(rb)) : ((LegManager)new LocalLegManager(rb, machineInput)));
			_legManagers[machineId] = value;
		}

		public void Tick(float deltaTime)
		{
			Dictionary<int, LegManager>.Enumerator enumerator = _legManagers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.Tick(deltaTime);
			}
		}

		public void PhysicsTick(float deltaTime)
		{
			Dictionary<int, LegManager>.Enumerator enumerator = _legManagers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.PhysicsTick(deltaTime);
			}
		}

		public void SetJumpStateForLocalLegs()
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
			if (_legManagers.TryGetValue(activeMachine, out LegManager value))
			{
				value.SetJumpState();
			}
		}

		internal void SetMachineLegsEnabled(int machineId, bool enabled)
		{
			LegManager legManager = _legManagers[machineId];
			if (enabled)
			{
				legManager.EnableLegs();
			}
			else
			{
				legManager.DisableLegs();
			}
		}

		public void RegisterLeg(CubeLeg leg, int machineId)
		{
			_legManagers[machineId].RegisterLeg(leg);
		}

		public void UnregisterLeg(CubeLeg leg, int machineId)
		{
			_legManagers[machineId].UnregisterLeg(leg);
		}
	}
}
