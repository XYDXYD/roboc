using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponAccuracyMonoBehaviour : MonoBehaviour, IWeaponAccuracyStatsComponent, IWeaponAccuracyModifierComponent, IWeaponMovementComponent, IImplementor
	{
		float IWeaponAccuracyStatsComponent.baseInAccuracyDegrees
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.baseAirInaccuracyDegrees
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.movementInAccuracyDegrees
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.movementMaxThresholdSpeed
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.movementMinThresholdSpeed
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.gunRotationThresholdSlow
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.movementInAccuracyDecayTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.slowRotationInAccuracyDecayTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.quickRotationInAccuracyDecayTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.movementInAccuracyRecoveryTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.repeatFireInAccuracyTotalDegrees
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.repeatFireInAccuracyDecayTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.repeatFireInAccuracyRecoveryTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.fireInstantAccuracyDecayDegrees
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.accuracyNonRecoverTime
		{
			get;
			set;
		}

		float IWeaponAccuracyStatsComponent.accuracyDecayTime
		{
			get;
			set;
		}

		float IWeaponAccuracyModifierComponent.totalAccuracy
		{
			get;
			set;
		}

		float IWeaponAccuracyModifierComponent.crosshairAccuracyModifier
		{
			get;
			set;
		}

		float IWeaponAccuracyModifierComponent.repeatFiringModifier
		{
			get;
			set;
		}

		float IWeaponAccuracyModifierComponent.movementAccuracyModifier
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.velocity
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.rotationVelocity
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.lastPosition
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.lastRotation
		{
			get;
			set;
		}

		public WeaponAccuracyMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
		}
	}
}
