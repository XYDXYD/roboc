using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	internal sealed class CubeHoverBladeGFX : MonoBehaviour, IHoverGFXComponent
	{
		public Transform spinBladeT;

		public Transform orientationT;

		public float minSpinSpeed;

		public float spinSpeedMultiplier;

		public Vector3 spinAxis;

		private Vector3 _thrustPos;

		private Vector3 _prevthrustPos;

		private Vector3 _currentLookAtOffset;

		public Vector3 currentLookAtOffset
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _currentLookAtOffset;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_currentLookAtOffset = value;
			}
		}

		public Transform orientation => orientationT;

		public Vector3 previousThrustPost
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _prevthrustPos;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_prevthrustPos = value;
			}
		}

		public float minSpinVel => minSpinSpeed;

		public float spinSpeedMult => spinSpeedMultiplier;

		public Vector3 spinAxisV => spinAxis;

		public Transform spinBlade => spinBladeT;

		public float currentSpinRotation
		{
			get;
			set;
		}

		public CubeHoverBladeGFX()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_currentLookAtOffset = Vector3.get_down();
		}
	}
}
