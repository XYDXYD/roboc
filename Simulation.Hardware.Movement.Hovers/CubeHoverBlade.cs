using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	internal sealed class CubeHoverBlade : MonoBehaviour, IMaxSpeedComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent, IHoverInfoComponent, IPartLevelComponent
	{
		public Transform thrustTransform;

		public int level = 1;

		private Vector3 _positiveAxisMaxSpeed = Vector3.get_one();

		private Vector3 _negativeAxisMaxSpeed = Vector3.get_one();

		private float _minItemsModifier = 1f;

		int IPartLevelComponent.level
		{
			get
			{
				return level;
			}
		}

		public Vector3 positiveAxisMaxSpeed
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _positiveAxisMaxSpeed;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_positiveAxisMaxSpeed = value;
			}
		}

		public Vector3 negativeAxisMaxSpeed
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _negativeAxisMaxSpeed;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_negativeAxisMaxSpeed = value;
			}
		}

		public float maxSpeed
		{
			get;
			set;
		}

		public float speedModifier => 1f;

		public float minItemsModifier
		{
			get
			{
				return _minItemsModifier;
			}
			set
			{
				_minItemsModifier = value;
			}
		}

		public bool isValid => validGroundScalar != 0f;

		public bool affectsMaxSpeed => true;

		public float maxCarryMass
		{
			get;
			set;
		}

		public Vector3 lastPos
		{
			get;
			set;
		}

		public int resetLastPosUpdates
		{
			get;
			set;
		}

		public float distanceToGround
		{
			get;
			set;
		}

		public float validGroundScalar
		{
			get;
			set;
		}

		public float initialYPos
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				Vector3 localPosition = this.get_transform().get_localPosition();
				return localPosition.y;
			}
		}

		public RaycastHit raycastHit
		{
			get;
			set;
		}

		public Transform forcePointTransform => thrustTransform;

		public CubeHoverBlade()
			: this()
		{
		}//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)

	}
}
