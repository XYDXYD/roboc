using System;
using UnityEngine;
using Utility;

namespace Simulation.Hardware.Weapons
{
	internal static class WeaponAimUtility
	{
		public static Vector3 GetDirection(IHardwareOwnerComponent ownerComponent, IAimingVariablesComponent aimingVariablesComponent, IWeaponRotationTransformsComponent weaponRotationTransformsComponent, IWeaponAccuracyModifierComponent weaponAccuracyComponent)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = (ownerComponent.ownedByAi || ownerComponent.ownedByMe) ? weaponRotationTransformsComponent.verticalTransform.get_forward() : aimingVariablesComponent.direction;
			if (ownerComponent.ownedByMe || ownerComponent.ownedByAi)
			{
				val = CalculateProjectileAccuracy(val, weaponAccuracyComponent.totalAccuracy);
			}
			return val;
		}

		private static Vector3 CalculateProjectileAccuracy(Vector3 in_dir, float accuracyModifier)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtility.RandomGausianDistribution();
			float num2 = MathUtility.RandomRotation();
			float num3 = Mathf.Tan(accuracyModifier * ((float)Math.PI / 180f));
			Vector3 val = CalculateNormalVector(in_dir);
			Vector3 val2 = Quaternion.AngleAxis(num2, in_dir) * val;
			Vector3 val3 = num3 * num * val2;
			Vector3 val4 = in_dir + val3;
			return val4.get_normalized();
		}

		private static Vector3 CalculateNormalVector(Vector3 dir)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(dir, Vector3.get_up());
			if (val.get_sqrMagnitude() < 0.1f)
			{
				val = Vector3.Cross(dir, Vector3.get_forward());
			}
			return val.get_normalized();
		}

		public static Vector3 GetMuzzlePosition(IWeaponMuzzleFlash muzzle)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (muzzle != null && muzzle.muzzleFlashLocations != null)
			{
				return muzzle.muzzleFlashLocations[muzzle.activeMuzzleFlash].get_position();
			}
			Console.LogException(new Exception("MuzzleFlashLocation not set up"));
			return Vector3.get_zero();
		}
	}
}
