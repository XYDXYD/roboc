using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponAimingMonoBehaviour : MonoBehaviour, IAimSpeedComponent, IMoveLimitsComponent, IWeaponMountingModeComponent, IWeaponRotationTransformsComponent, IAimingVariablesComponent, IImplementor
	{
		public Transform horizontalTransform;

		public Transform verticalTransform;

		public Transform additionalVerticalTransform;

		public float aimSpeed = 6f;

		public float minHorizAngle = -180f;

		public float maxHorizAngle = 180f;

		public float minVerticalAngle = -18f;

		public float maxVerticalAngle = 90f;

		public float secondVerticalJointAngle;

		public bool _enableWorldSpaceLimit;

		public float _maxVerticalAngleWorld;

		public float _minVerticalAngleWorld;

		public float _verticalAngleOffset;

		public float _verticalAngleMultiplier = 1f;

		public bool _aimToPoint = true;

		public MountMode mountingMode;

		float IAimSpeedComponent.aimSpeed
		{
			get
			{
				return aimSpeed;
			}
		}

		float IMoveLimitsComponent.minHorizAngle
		{
			get
			{
				return minHorizAngle;
			}
		}

		float IMoveLimitsComponent.maxHorizAngle
		{
			get
			{
				return maxHorizAngle;
			}
		}

		float IMoveLimitsComponent.minVerticalAngle
		{
			get
			{
				return minVerticalAngle;
			}
		}

		float IMoveLimitsComponent.maxVerticalAngle
		{
			get
			{
				return maxVerticalAngle;
			}
		}

		float IMoveLimitsComponent.secondVerticalJointAngle
		{
			get
			{
				return secondVerticalJointAngle;
			}
		}

		MountMode IWeaponMountingModeComponent.mountingMode
		{
			get
			{
				return mountingMode;
			}
		}

		Transform IWeaponRotationTransformsComponent.horizontalTransform
		{
			get
			{
				return horizontalTransform;
			}
		}

		Transform IWeaponRotationTransformsComponent.verticalTransform
		{
			get
			{
				return verticalTransform;
			}
		}

		Transform IWeaponRotationTransformsComponent.secondVerticalTransform
		{
			get
			{
				return additionalVerticalTransform;
			}
		}

		float IAimingVariablesComponent.sqrRotationVelocity
		{
			get;
			set;
		}

		float IAimingVariablesComponent.targetHorizAngle
		{
			get;
			set;
		}

		float IAimingVariablesComponent.targetVertAngle
		{
			get;
			set;
		}

		float IAimingVariablesComponent.currHorizAngle
		{
			get;
			set;
		}

		float IAimingVariablesComponent.currVertAngle
		{
			get;
			set;
		}

		bool IAimingVariablesComponent.largeAimOffset
		{
			get;
			set;
		}

		bool IAimingVariablesComponent.changingAimQuickly
		{
			get;
			set;
		}

		Vector3 IAimingVariablesComponent.targetPoint
		{
			get;
			set;
		}

		Vector3 IAimingVariablesComponent.direction
		{
			get;
			set;
		}

		bool IAimingVariablesComponent.isBlocked
		{
			get;
			set;
		}

		public Quaternion initialHorizRot
		{
			get;
			set;
		}

		public Quaternion initialVertRot
		{
			get;
			set;
		}

		public bool enableWorldSpaceLimit => _enableWorldSpaceLimit;

		public float maxVerticalAngleWorld => _maxVerticalAngleWorld;

		public float minVerticalAngleWorld => _minVerticalAngleWorld;

		public float verticalAngleOffset => _verticalAngleOffset;

		public float verticalAngleMultiplier => _verticalAngleMultiplier;

		public bool aimToPoint => _aimToPoint;

		public WeaponAimingMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			initialHorizRot = horizontalTransform.get_localRotation();
			if (verticalTransform != null)
			{
				initialVertRot = verticalTransform.get_localRotation();
			}
		}
	}
}
