using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.BattleArena
{
	public class InsideFusionShieldEngine : IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		[Inject]
		internal FusionShieldsObserver shieldsObserver
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColliderIgnoreObservable machineColliderIgnoreObservable
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			shieldsObserver.RegisterShieldStateChanged(OnShieldStateChanged);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		public void OnFrameworkDestroyed()
		{
			shieldsObserver.UnregisterShieldStateChanged(OnShieldStateChanged);
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				UpdateIsInsideShieldData();
				yield return null;
			}
		}

		private void UpdateIsInsideShieldData()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<InsideFusionShieldEntityView> val = entityViewsDB.QueryEntityViews<InsideFusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				InsideFusionShieldEntityView insideFusionShieldEntityView = val.get_Item(i);
				if (insideFusionShieldEntityView.aliveStateComponent.isAlive.get_value())
				{
					ProcessPlayerMachine(insideFusionShieldEntityView);
				}
			}
		}

		private void ProcessPlayerMachine(InsideFusionShieldEntityView playerMachine)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			FasterReadOnlyList<FusionShieldEntityView> val = entityViewsDB.QueryEntityViews<FusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				FusionShieldEntityView fusionShieldEntityView = val.get_Item(i);
				if (IsMachineNearShield(fusionShieldEntityView, playerMachine) && (IsMachineInsideFusionShield(fusionShieldEntityView, playerMachine) || IsAnyWeaponInsideFusionShield(fusionShieldEntityView, playerMachine) || IsAnyWeaponCollisionInsideFusionShield(fusionShieldEntityView, playerMachine)))
				{
					flag = true;
					playerMachine.insideFusionShieldComponent.isInsideShield = true;
					playerMachine.insideFusionShieldComponent.teamId = fusionShieldEntityView.ownerTeamComponent.ownerTeamId;
				}
			}
			if (playerMachine.insideFusionShieldComponent.isInsideShield && !flag)
			{
				playerMachine.insideFusionShieldComponent.isInsideShield = false;
				playerMachine.insideFusionShieldComponent.isEncapsulated = false;
				UpdateMachineColliderIgnores(playerMachine);
			}
		}

		private bool IsMachineNearShield(FusionShieldEntityView fusionShield, InsideFusionShieldEntityView playerMachine)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = fusionShield.transformComponent.T.get_position();
			FusionShieldCollider enterRangeShieldCollider = fusionShield.fusionShieldColliderComponent.enterRangeShieldCollider;
			Vector3 worldCenterOfMass = playerMachine.rigidBodyComponent.rb.get_worldCenterOfMass();
			Vector3 val = position + enterRangeShieldCollider.localCenter - worldCenterOfMass;
			val.y = 0f;
			return val.get_sqrMagnitude() < enterRangeShieldCollider.radiusSq;
		}

		private bool IsMachineInsideFusionShield(FusionShieldEntityView fusionShield, InsideFusionShieldEntityView playerMachine)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Vector3 worldCenterOfMass = playerMachine.rigidBodyComponent.rb.get_worldCenterOfMass();
			return IsInsideFusionShield(fusionShield, worldCenterOfMass);
		}

		private bool IsAnyWeaponInsideFusionShield(FusionShieldEntityView fusionShield, InsideFusionShieldEntityView playerMachine)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<BlockWeaponFireNode> val = entityViewsDB.QueryEntityViews<BlockWeaponFireNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				BlockWeaponFireNode blockWeaponFireNode = val.get_Item(i);
				if (blockWeaponFireNode.weaponOwnerComponent.machineId == playerMachine.machineOwnerComponent.ownerMachineId && !blockWeaponFireNode.weaponHealthStatusComponent.disabled)
				{
					IWeaponMuzzleFlash muzzleFlashComponent = blockWeaponFireNode.muzzleFlashComponent;
					Vector3 muzzlePosition = WeaponAimUtility.GetMuzzlePosition(muzzleFlashComponent);
					if (IsInsideFusionShield(fusionShield, muzzlePosition))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsAnyWeaponCollisionInsideFusionShield(FusionShieldEntityView fusionShield, InsideFusionShieldEntityView playerMachine)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<BlockWeaponCollisionNode> val = entityViewsDB.QueryEntityViews<BlockWeaponCollisionNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				BlockWeaponCollisionNode blockWeaponCollisionNode = val.get_Item(i);
				if (blockWeaponCollisionNode.weaponOwnerComponent.machineId == playerMachine.machineOwnerComponent.ownerMachineId && !blockWeaponCollisionNode.weaponHealthStatusComponent.disabled)
				{
					Vector3 position = blockWeaponCollisionNode.transformComponent.T.get_position();
					if (IsInsideFusionShield(fusionShield, position))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsInsideFusionShield(FusionShieldEntityView fusionShield, Vector3 pos)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = fusionShield.transformComponent.T.get_position();
			FusionShieldCollider shieldCapsuleCollider = fusionShield.fusionShieldColliderComponent.shieldCapsuleCollider;
			Vector3 val = position + shieldCapsuleCollider.localCenter - pos;
			val.y = 0f;
			if (val.get_sqrMagnitude() > shieldCapsuleCollider.radiusSq)
			{
				return false;
			}
			FusionShieldCollider[] shieldSphereColliders = fusionShield.fusionShieldColliderComponent.shieldSphereColliders;
			if (shieldSphereColliders != null)
			{
				for (int i = 0; i < shieldSphereColliders.Length; i++)
				{
					val = position + shieldSphereColliders[i].localCenter - pos;
					if (val.get_sqrMagnitude() > shieldSphereColliders[i].radiusSq)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private void OnShieldStateChanged(int teamId, bool fullPower)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			FusionShieldEntityView fusionShieldEntityView = entityViewsDB.QueryEntityView<FusionShieldEntityView>(teamId);
			FasterReadOnlyList<InsideFusionShieldEntityView> val = entityViewsDB.QueryEntityViews<InsideFusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				InsideFusionShieldEntityView insideFusionShieldEntityView = val.get_Item(i);
				if (insideFusionShieldEntityView.insideFusionShieldComponent.isInsideShield && insideFusionShieldEntityView.insideFusionShieldComponent.teamId == teamId)
				{
					insideFusionShieldEntityView.insideFusionShieldComponent.isEncapsulated = fullPower;
					UpdateMachineColliderIgnores(insideFusionShieldEntityView);
				}
			}
		}

		private void UpdateMachineColliderIgnores(InsideFusionShieldEntityView playerMachine)
		{
			if (playerMachine.insideFusionShieldComponent.teamId != playerMachine.ownerTeamComponent.ownerTeamId)
			{
				int ownerMachineId = playerMachine.machineOwnerComponent.ownerMachineId;
				machineColliderIgnoreObservable.Dispatch(ref ownerMachineId);
			}
		}
	}
}
