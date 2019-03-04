using Svelto.IoC;
using System;
using UnityEngine;
using Utility;

namespace Simulation.Hardware.Weapons
{
	internal sealed class RemoteClientWeaponFire
	{
		[Inject]
		internal LivePlayersContainer livePlayersContainer
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
		internal RigidbodyDataContainer rigidbodyDataContainer
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
		internal NetworkHitEffectObservable fireObservable
		{
			private get;
			set;
		}

		public void FireRemoteClientWeapons(TargetType type, Byte3 hitCube, bool targetIsMe, ItemDescriptor itemDescriptor, int shootingMachineId, int hitMachineId, Vector3 hitEffectOffset, Vector3 hitEffectNormal, int stackCount = 0)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (playerMachinesContainer.IsMachineRegistered(type, hitMachineId))
			{
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, shootingMachineId);
				bool flag = playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId);
				int playerFromMachineId2 = playerMachinesContainer.GetPlayerFromMachineId(type, hitMachineId);
				bool flag2 = !livePlayersContainer.IsPlayerAlive(type, playerFromMachineId2);
				if (!flag && !flag2)
				{
					try
					{
						Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(type, hitMachineId);
						Vector3 val = rigidBodyData.get_position() + rigidBodyData.get_rotation() * GridScaleUtility.GridToWorld(hitCube, type) + hitEffectOffset;
						Quaternion rotation_ = Quaternion.LookRotation(hitEffectNormal);
						bool flag3 = !playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId);
						HitInfo hitInfo = new HitInfo(type, itemDescriptor, flag3, hit_: true, hitSelf_: false, val, rotation_, hitEffectNormal, targetIsMe, flag, !flag3, rigidBodyData, stackCount);
						fireObservable.Dispatch(ref hitInfo);
						if (targetIsMe)
						{
							ApplyImpactForce(rigidBodyData, -hitEffectNormal, val);
						}
					}
					catch (Exception ex)
					{
						Console.LogException(ex);
					}
				}
			}
		}

		private void ApplyImpactForce(Rigidbody rb, Vector3 direction, Vector3 hitPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			ApplyImpactForceToRigidBodyCommand applyImpactForceToRigidBodyCommand = new ApplyImpactForceToRigidBodyCommand(rb, direction, direction.get_magnitude(), hitPosition);
			applyImpactForceToRigidBodyCommand.Execute();
		}
	}
}
