using System;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal sealed class WheelGraphicsComponentImplementor : MonoBehaviour, IWheelColliderInfo, IWheelRadiusComponent, IPreviousStateComponent, IGraphicsTransformComponent, IWheelSpeedComponent, IWheelSuspensionComponent, IWheelRotationAxisComponent
	{
		public Transform wheelToRotate;

		public Transform wheelVerticalPosition;

		public Transform steeringNode;

		public Vector3 rotationAxis = Vector3.get_forward();

		Vector3 IWheelRotationAxisComponent.rotationAxis
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return rotationAxis;
			}
		}

		public float wheelRadius
		{
			get;
			set;
		}

		public float inverseCircumference
		{
			get;
			set;
		}

		public float prevSpeed
		{
			get;
			set;
		}

		public Vector3 prevPosition
		{
			get;
			set;
		}

		public Quaternion prevRotation
		{
			get;
			set;
		}

		public Transform suspensionTransform => wheelVerticalPosition;

		public Transform wheelToRotateTransform => wheelToRotate;

		public Transform steeringNodeTransform => steeringNode;

		public float wheelSpeed
		{
			get;
			set;
		}

		public float fullSuspensionDistance
		{
			get;
			set;
		}

		public float wheelForwardOffset
		{
			get;
			set;
		}

		public WheelGraphicsComponentImplementor()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)


		public void SetWheelColliderInfo(WheelColliderData data)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			Vector3 lossyScale = data.wheelObj.get_lossyScale();
			float x = lossyScale.x;
			float num = data.suspensionDistance * x;
			wheelRadius = data.radius * x;
			if (wheelRadius > 0f)
			{
				inverseCircumference = 1f / ((float)Math.PI * 2f * wheelRadius);
			}
			fullSuspensionDistance = wheelRadius + num;
			wheelForwardOffset = Vector3.Dot(wheelVerticalPosition.get_position() - data.cubeRoot.get_position(), data.cubeRoot.get_up());
		}

		public void WheelColliderActivated()
		{
		}
	}
}
