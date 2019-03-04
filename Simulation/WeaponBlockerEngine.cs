using Simulation.BattleArena;
using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	internal class WeaponBlockerEngine : IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, ItemCategory> _machineItemCategory = new Dictionary<int, ItemCategory>();

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		[Inject]
		internal FusionShieldsObserver shieldsObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(OnGameStart);
			switchWeaponObserver.SwitchWeaponsEvent += SwitchToNewWeaponCategory;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
			switchWeaponObserver.SwitchWeaponsEvent -= SwitchToNewWeaponCategory;
		}

		public void Ready()
		{
		}

		private void SwitchToNewWeaponCategory(int machineId, ItemDescriptor itemDescriptor)
		{
			_machineItemCategory[machineId] = itemDescriptor.itemCategory;
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<MachineWeaponsBlockedNode> playerMachines = entityViewsDB.QueryEntityViews<MachineWeaponsBlockedNode>();
				for (int i = 0; i < playerMachines.get_Count(); i++)
				{
					MachineWeaponsBlockedNode machineWeaponsBlockedNode = playerMachines.get_Item(i);
					bool flag = IsBlockedByFusionshield(machineWeaponsBlockedNode);
					machineWeaponsBlockedNode.machineWeaponsBlockedComponent.blockedByFusionShield = flag;
					bool flag2 = IsWeaponBlockedBecauseNotGrounded(machineWeaponsBlockedNode);
					machineWeaponsBlockedNode.machineWeaponsBlockedComponent.weaponNotGrounded = flag2;
					flag |= flag2;
					machineWeaponsBlockedNode.machineWeaponsBlockedComponent.blocked = flag;
				}
				yield return null;
			}
		}

		private bool IsWeaponBlockedBecauseNotGrounded(MachineWeaponsBlockedNode playerMachine)
		{
			int ownerMachineId = playerMachine.machineOwnerComponent.ownerMachineId;
			MachineGroundedNode machineGroundedNode = default(MachineGroundedNode);
			if (entityViewsDB.TryQueryEntityView<MachineGroundedNode>(ownerMachineId, ref machineGroundedNode) && !machineGroundedNode.machineGroundedComponent.grounded && _machineItemCategory.TryGetValue(ownerMachineId, out ItemCategory value))
			{
				return value == ItemCategory.Aeroflak || value == ItemCategory.Mortar;
			}
			return false;
		}

		private bool IsBlockedByFusionshield(MachineWeaponsBlockedNode playerMachine)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<FusionShieldEntityView> val = entityViewsDB.QueryEntityViews<FusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				FusionShieldEntityView fusionShieldEntityView = val.get_Item(i);
				InsideFusionShieldEntityView insideFusionShieldEntityView = default(InsideFusionShieldEntityView);
				if (playerMachine.ownerTeamComponent.ownerTeamId != fusionShieldEntityView.ownerTeamComponent.ownerTeamId && fusionShieldEntityView.activableComponent.powerState && entityViewsDB.TryQueryEntityView<InsideFusionShieldEntityView>(playerMachine.machineOwnerComponent.ownerMachineId, ref insideFusionShieldEntityView) && insideFusionShieldEntityView.insideFusionShieldComponent.isInsideShield && insideFusionShieldEntityView.insideFusionShieldComponent.teamId != playerMachine.ownerTeamComponent.ownerTeamId)
				{
					return true;
				}
			}
			return false;
		}

		private void OnGameStart()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Update);
		}
	}
}
