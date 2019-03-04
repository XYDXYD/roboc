using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal class SkiGraphicsImplementor : MonoBehaviour, IGraphicsTransformComponent, ISkiSuspensionComponent, ISkiRaycastComponent, ISkiHingeComponent, IPreviousStateComponent, ISpeedComponent
	{
		public Transform steeringNode;

		public Transform suspensionNode;

		public Transform suspensionHinge;

		public Vector3 compressedOffset_;

		public Vector3 extendedOffset_;

		public Transform raycastPoint_;

		public float raycastOffset_ = 1f;

		public float raycastDist_ = 1f;

		public float raycastBackTrack_ = 0.2f;

		private float suspensionTravel;

		public Transform suspensionTransform => suspensionNode;

		public Transform hingeTransform => suspensionHinge;

		public Transform steeringNodeTransform => steeringNode;

		public Vector3 compressedOffset => compressedOffset_;

		public Vector3 extendedOffset => extendedOffset_;

		public float travel
		{
			get;
			set;
		}

		public Vector3 localPosition
		{
			get;
			set;
		}

		public Transform raycastPoint => raycastPoint_;

		public float raycastOffset => raycastOffset_;

		public float raycastDist => raycastDist_;

		public float raycastBackTrack => raycastBackTrack_;

		public Quaternion currentRotation
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

		public float currentSpeed
		{
			get;
			set;
		}

		public bool movingBackwards
		{
			get;
			set;
		}

		public SkiGraphicsImplementor()
			: this()
		{
		}

		private void Start()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Vector3 lossyScale = suspensionNode.get_lossyScale();
			float x = lossyScale.x;
			Vector3 val = extendedOffset_ - compressedOffset_;
			travel = Mathf.Clamp(val.get_magnitude() * x, 0.001f, raycastDist);
		}
	}
}
