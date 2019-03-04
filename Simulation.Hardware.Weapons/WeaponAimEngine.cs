using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponAimEngine : SingleEntityViewEngine<WeaponAimNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		protected override void Add(WeaponAimNode node)
		{
			IAimingVariablesComponent weaponAimingComponent = node.weaponAimingComponent;
			if (node.moveLimitsComponent.verticalAngleOffset != 0f)
			{
				weaponAimingComponent.currVertAngle = (0f - node.moveLimitsComponent.verticalAngleOffset) * ((float)Math.PI / 180f);
				weaponAimingComponent.targetVertAngle = weaponAimingComponent.currHorizAngle;
				SetTransformAngles(node.weaponRotationTransformsComponent, weaponAimingComponent);
			}
		}

		protected override void Remove(WeaponAimNode obj)
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MachineRaycastView> val = entityViewsDB.QueryEntityViews<MachineRaycastView>();
			for (int num = val.get_Count() - 1; num >= 0; num--)
			{
				MachineRaycastView machineRaycastView = val.get_Item(num);
				if (!machineRaycastView.stunComponent.stunned)
				{
					int num2 = default(int);
					WeaponAimNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<WeaponAimNode>(machineRaycastView.get_ID(), ref num2);
					for (int i = 0; i < num2; i++)
					{
						WeaponAimNode weaponAimNode = array[i];
						if (!weaponAimNode.healthStatusComponent.disabled && weaponAimNode.weaponActiveComponent.active && (!weaponAimNode.visibilityComponent.offScreen || weaponAimNode.ownerComponent.ownedByAi))
						{
							WeaponRaycast weaponRaycast = machineRaycastView.raycastComponent.weaponRaycast;
							weaponRaycast.SetMaxRange(weaponAimNode.weaponRangeComponent.maxRange);
							weaponAimNode.weaponAimingComponent.targetPoint = weaponRaycast.targetPoint;
							bool isBlockedVert = false;
							Vector3 aimDirection = GetAimDirection(weaponAimNode, weaponRaycast, out isBlockedVert);
							AimWeapon(weaponAimNode, aimDirection, isBlockedVert);
						}
					}
				}
			}
		}

		private static Vector3 GetAimDirection(WeaponAimNode node, WeaponRaycast weaponRaycast, out bool isBlockedVert)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			isBlockedVert = false;
			Vector3 aimPoint = weaponRaycast.aimPoint;
			Vector3 val = (!node.weaponAimingComponent.aimToPoint && node.ownerComponent.ownedByMe) ? weaponRaycast.GetHitFreeForward() : (aimPoint - node.weaponRotationTransformsComponent.verticalTransform.get_position());
			IMoveLimitsComponent moveLimitsComponent = node.moveLimitsComponent;
			float verticalAngleOffset = moveLimitsComponent.verticalAngleOffset;
			float verticalAngleMultiplier = moveLimitsComponent.verticalAngleMultiplier;
			if (verticalAngleOffset != 0f || verticalAngleMultiplier != 1f)
			{
				val = ApplyVerticalAimOffset(val, verticalAngleOffset, verticalAngleMultiplier, ref isBlockedVert);
			}
			if (moveLimitsComponent.enableWorldSpaceLimit)
			{
				val = ApplyWorldSpaceLimit(val, moveLimitsComponent.maxVerticalAngleWorld, ref isBlockedVert, moveLimitsComponent.minVerticalAngleWorld);
			}
			return val;
		}

		private static Vector3 ApplyVerticalAimOffset(Vector3 targetVectorWorld, float verticalOffset, float multiplier, ref bool isBlockedVert)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Vector3 normalized = targetVectorWorld.get_normalized();
			float num = 57.29578f * Mathf.Asin(normalized.y);
			if (num > 0f)
			{
				verticalOffset += num * (multiplier - 1f);
			}
			Vector3 val = Vector3.Cross(normalized, Vector3.get_up());
			float num2 = verticalOffset;
			if (num + num2 > 89f)
			{
				num2 = 89f - num;
				isBlockedVert = true;
			}
			return Quaternion.AngleAxis(num2, val) * targetVectorWorld;
		}

		private static Vector3 ApplyWorldSpaceLimit(Vector3 targetVectorWorld, float maxVerticalAngleWorld, ref bool isBlockedVert, float minVerticalAngleWorld)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = targetVectorWorld.get_normalized();
			float num = 57.29578f * Mathf.Asin(val.y);
			if (num > maxVerticalAngleWorld)
			{
				float num2 = maxVerticalAngleWorld - num;
				Vector3 val2 = Vector3.Cross(val, Vector3.get_up());
				val = Quaternion.AngleAxis(num2, val2) * val;
				isBlockedVert = true;
			}
			else if (num < minVerticalAngleWorld)
			{
				float num3 = minVerticalAngleWorld - num;
				Vector3 val3 = Vector3.Cross(val, Vector3.get_up());
				val = Quaternion.AngleAxis(num3, val3) * val;
				isBlockedVert = true;
			}
			return val;
		}

		private void AimWeapon(WeaponAimNode node, Vector3 aimDirection, bool isBlockedVert)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			MountMode mountingMode = node.weaponMountingModeComponent.mountingMode;
			IWeaponRotationTransformsComponent weaponRotationTransformsComponent = node.weaponRotationTransformsComponent;
			IMoveLimitsComponent moveLimitsComponent = node.moveLimitsComponent;
			IAimingVariablesComponent weaponAimingComponent = node.weaponAimingComponent;
			Transform t = node.transformComponent.T;
			Vector3 angularVelocity = node.rigidBodyComponent.rb.get_angularVelocity();
			float aimSpeed = node.aimSpeedComponent.aimSpeed;
			weaponAimingComponent.largeAimOffset = false;
			weaponAimingComponent.changingAimQuickly = false;
			bool isBlockedHoriz = false;
			switch (mountingMode)
			{
			case MountMode.MountVertical:
				AimWeaponVerticalMount(aimDirection, aimSpeed, t, weaponRotationTransformsComponent, moveLimitsComponent, weaponAimingComponent, out isBlockedHoriz, out isBlockedVert);
				break;
			case MountMode.MountHorizontal:
				AimWeaponHorizontalMount(aimDirection, aimSpeed, t, weaponRotationTransformsComponent, moveLimitsComponent, weaponAimingComponent, out isBlockedHoriz, out isBlockedVert);
				break;
			}
			UpdateWeaponAim(aimSpeed, moveLimitsComponent, weaponAimingComponent, isBlockedHoriz, isBlockedVert, angularVelocity.y);
			SetTransformAngles(weaponRotationTransformsComponent, weaponAimingComponent);
		}

		private void AimWeaponVerticalMount(Vector3 aimDirectionWorld, float aimSpeed, Transform T, IWeaponRotationTransformsComponent weaponRotationTransform, IMoveLimitsComponent moveLimits, IAimingVariablesComponent aimingComponent, out bool isBlockedHoriz, out bool isBlockedVert)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			Quaternion rotation = T.get_rotation();
			Quaternion val = Quaternion.Inverse(rotation);
			Vector3 val2 = val * aimDirectionWorld;
			float num = aimSpeed * Time.get_deltaTime() * ((float)Math.PI / 180f);
			isBlockedHoriz = false;
			isBlockedVert = false;
			if (weaponRotationTransform.horizontalTransform != null)
			{
				Vector3 val3 = val2;
				val3.y = 0f;
				val3.Normalize();
				float value = Vector3.Dot(val3, Vector3.get_right());
				value = Math.Sign(value);
				float num2 = Vector3.Dot(val3, Vector3.get_forward());
				num2 = Mathf.Clamp(num2, -1f, 1f);
				float num3 = Mathf.Acos(num2) * value;
				float num4 = moveLimits.minHorizAngle * ((float)Math.PI / 180f);
				float num5 = moveLimits.maxHorizAngle * ((float)Math.PI / 180f);
				float num6 = Mathf.Abs(num3 - aimingComponent.currHorizAngle);
				aimingComponent.largeAimOffset |= (num6 > num);
				isBlockedHoriz |= (num3 < num4 || num3 >= num5);
				num3 = (aimingComponent.targetHorizAngle = Mathf.Clamp(num3, num4, num5));
			}
			if (weaponRotationTransform.verticalTransform != null)
			{
				Vector3 val4 = val2;
				Vector3 val5 = val4;
				val5.y = 0f;
				val5.Normalize();
				val4.Normalize();
				float num8 = Vector3.Dot(Vector3.get_up(), val4);
				num8 = Mathf.Sign(num8);
				float num9 = Vector3.Dot(val4, val5);
				num9 = Mathf.Clamp(num9, -1f, 1f);
				float num10 = Mathf.Acos(num9) * num8;
				float num11 = moveLimits.minVerticalAngle * ((float)Math.PI / 180f);
				float num12 = moveLimits.maxVerticalAngle * ((float)Math.PI / 180f);
				float num13 = Mathf.Abs(0f - num10 - aimingComponent.currVertAngle);
				aimingComponent.largeAimOffset |= (num13 > num);
				isBlockedVert |= (num10 < num11 || num10 >= num12);
				num10 = Mathf.Clamp(num10, num11, num12);
				num10 = (aimingComponent.targetVertAngle = num10 * -1f);
			}
		}

		private void AimWeaponHorizontalMount(Vector3 aimDirectionWorld, float aimSpeed, Transform T, IWeaponRotationTransformsComponent weaponRotationTransform, IMoveLimitsComponent moveLimits, IAimingVariablesComponent aimingComponent, out bool isBlockedHoriz, out bool isBlockedVert)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = Quaternion.Inverse(T.get_rotation());
			Vector3 val2 = val * aimDirectionWorld;
			isBlockedHoriz = false;
			isBlockedVert = false;
			float num = aimSpeed * Time.get_deltaTime() * ((float)Math.PI / 180f);
			if (weaponRotationTransform.horizontalTransform != null)
			{
				Vector3 val3 = val2;
				val3.z = 0f;
				val3.Normalize();
				Vector3 val4 = val3;
				val4.x = 0f;
				val4.Normalize();
				float num2 = Vector3.Angle(val3, val4);
				float num3 = Vector3.Dot(val3, Vector3.get_right());
				num3 = Mathf.Sign(num3) * -1f;
				float num4 = num2 * num3;
				float num5 = Mathf.Abs(num4 * ((float)Math.PI / 180f) - aimingComponent.currHorizAngle);
				aimingComponent.largeAimOffset |= (num5 > num);
				isBlockedHoriz |= (num4 < moveLimits.minHorizAngle || num4 >= moveLimits.maxHorizAngle);
				num2 = Mathf.Clamp(num4, moveLimits.minHorizAngle, moveLimits.maxHorizAngle);
				num2 = (aimingComponent.targetHorizAngle = num2 * ((float)Math.PI / 180f));
			}
			if (weaponRotationTransform.verticalTransform != null)
			{
				Vector3 val5 = val2;
				val5.Normalize();
				Vector3 val6 = Vector3.ProjectOnPlane(val5, Vector3.get_forward());
				val6.Normalize();
				float num7 = Vector3.Angle(val5, val6);
				float num8 = Vector3.Dot(val5, Vector3.get_forward());
				num8 = Mathf.Sign(num8) * -1f;
				float num9 = num7 * num8;
				float num10 = Mathf.Abs(num9 * ((float)Math.PI / 180f) - aimingComponent.currVertAngle);
				aimingComponent.largeAimOffset |= (num10 > num);
				isBlockedVert |= (num9 < moveLimits.minVerticalAngle || num9 >= moveLimits.maxVerticalAngle);
				num7 = Mathf.Clamp(num9, moveLimits.minVerticalAngle, moveLimits.maxVerticalAngle);
				num7 = (aimingComponent.targetVertAngle = num7 * ((float)Math.PI / 180f));
			}
		}

		private void UpdateWeaponAim(float aimSpeed, IMoveLimitsComponent moveLimits, IAimingVariablesComponent aimingComponent, bool isBlockedHoriz, bool isBlockedVert, float robotsRotationSpeedAboutY)
		{
			float num = aimSpeed * Time.get_deltaTime() * ((float)Math.PI / 180f);
			float num2 = (float)Math.PI * 2f * Mathf.Sign(aimingComponent.targetHorizAngle);
			float num3 = (float)Math.PI * 2f * Mathf.Sign(aimingComponent.targetVertAngle);
			bool flag = moveLimits.maxHorizAngle - moveLimits.minHorizAngle > 359f;
			bool flag2 = moveLimits.maxVerticalAngle - moveLimits.minVerticalAngle > 359f;
			aimingComponent.currHorizAngle -= robotsRotationSpeedAboutY * Time.get_deltaTime();
			float num4 = aimingComponent.targetHorizAngle - aimingComponent.currHorizAngle;
			if (flag)
			{
				num2 = aimingComponent.targetHorizAngle - num2 - aimingComponent.currHorizAngle;
				if (Mathf.Abs(num2) < Mathf.Abs(num4))
				{
					num4 = num2;
				}
			}
			if (num4 < 0f - num || num4 > num)
			{
				num4 = num * Mathf.Sign(num4);
				aimingComponent.changingAimQuickly = true;
			}
			float num5 = aimingComponent.targetVertAngle - aimingComponent.currVertAngle;
			if (flag2)
			{
				num3 = aimingComponent.targetVertAngle - num3 - aimingComponent.currVertAngle;
				if (Mathf.Abs(num3) < Mathf.Abs(num5))
				{
					num5 = num3;
				}
			}
			if (num5 < 0f - num || num5 > num)
			{
				num5 = num * Mathf.Sign(num5);
				aimingComponent.changingAimQuickly = true;
			}
			aimingComponent.sqrRotationVelocity = (num4 * num4 + num5 * num5) / (Time.get_deltaTime() * Time.get_deltaTime()) * 57.29578f * 57.29578f;
			aimingComponent.currHorizAngle += num4;
			aimingComponent.currVertAngle += num5;
			if (Mathf.Abs(aimingComponent.currHorizAngle) > (float)Math.PI)
			{
				aimingComponent.currHorizAngle -= (float)Math.PI * 2f * Mathf.Sign(aimingComponent.currHorizAngle);
			}
			if (Mathf.Abs(aimingComponent.currVertAngle) > (float)Math.PI)
			{
				aimingComponent.currVertAngle -= (float)Math.PI * 2f * Mathf.Sign(aimingComponent.currVertAngle);
			}
			bool flag3 = Mathf.Abs(num4) > 0.001f;
			bool flag4 = Mathf.Abs(num5) > 0.001f;
			bool flag6 = aimingComponent.isBlocked = ((!flag3 && isBlockedHoriz) || (!flag4 && isBlockedVert));
		}

		private void SetTransformAngles(IWeaponRotationTransformsComponent weaponRotationTransform, IAimingVariablesComponent aimingComponent)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			if (weaponRotationTransform.horizontalTransform != null)
			{
				weaponRotationTransform.horizontalTransform.set_localRotation(aimingComponent.initialHorizRot);
			}
			if (weaponRotationTransform.verticalTransform != null)
			{
				weaponRotationTransform.verticalTransform.set_localRotation(aimingComponent.initialVertRot);
			}
			if (weaponRotationTransform.secondVerticalTransform != null)
			{
				weaponRotationTransform.secondVerticalTransform.set_localRotation(aimingComponent.initialVertRot);
			}
			if (weaponRotationTransform.horizontalTransform != null)
			{
				weaponRotationTransform.horizontalTransform.Rotate(weaponRotationTransform.horizontalTransform.get_up(), aimingComponent.currHorizAngle * 57.29578f, 0);
			}
			if (!(weaponRotationTransform.verticalTransform != null))
			{
				return;
			}
			Transform verticalTransform = weaponRotationTransform.verticalTransform;
			Transform secondVerticalTransform = weaponRotationTransform.secondVerticalTransform;
			float num = aimingComponent.currVertAngle * 57.29578f;
			Vector3 right = weaponRotationTransform.verticalTransform.get_right();
			if (secondVerticalTransform != null)
			{
				float num2 = 0f;
				float num3;
				float num4;
				if (num > num2)
				{
					num3 = num2;
					num4 = num;
				}
				else
				{
					num3 = num;
					num4 = num2;
				}
				verticalTransform.Rotate(right, num3, 0);
				secondVerticalTransform.Rotate(right, num4, 0);
			}
			else
			{
				verticalTransform.Rotate(right, num, 0);
			}
		}

		public void Ready()
		{
		}
	}
}
