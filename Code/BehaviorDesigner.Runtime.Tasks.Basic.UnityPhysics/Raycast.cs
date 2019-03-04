using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPhysics
{
	[TaskCategory("Basic/Physics")]
	[TaskDescription("Casts a ray against all colliders in the scene. Returns success if a collider was hit.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=117")]
	public class Raycast : Action
	{
		[Tooltip("Starts the ray at the GameObject's position. If null the originPosition will be used")]
		public SharedGameObject originGameObject;

		[Tooltip("Starts the ray at the position. Only used if originGameObject is null")]
		public SharedVector3 originPosition;

		[Tooltip("The direction of the ray")]
		public SharedVector3 direction;

		[Tooltip("The length of the ray. Set to -1 for infinity")]
		public SharedFloat distance = -1f;

		[Tooltip("Selectively ignore colliders")]
		public LayerMask layerMask = LayerMask.op_Implicit(-1);

		[Tooltip("Cast the ray in world or local space. The direction is in world space if no GameObject is specified")]
		public Space space = 1;

		[SharedRequired]
		[Tooltip("Stores the hit object of the raycast")]
		public SharedGameObject storeHitObject;

		[SharedRequired]
		[Tooltip("Stores the hit point of the raycast")]
		public SharedVector3 storeHitPoint;

		[SharedRequired]
		[Tooltip("Stores the hit normal of the raycast")]
		public SharedVector3 storeHitNormal;

		[SharedRequired]
		[Tooltip("Stores the hit distance of the raycast")]
		public SharedFloat storeHitDistance;

		public Raycast()
			: this()
		{
		}//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)


		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = direction.get_Value();
			Vector3 val2;
			if (originGameObject.get_Value() != null)
			{
				val2 = originGameObject.get_Value().get_transform().get_position();
				if ((int)space == 1)
				{
					val = originGameObject.get_Value().get_transform().TransformDirection(direction.get_Value());
				}
			}
			else
			{
				val2 = originPosition.get_Value();
			}
			RaycastHit val3 = default(RaycastHit);
			if (Physics.Raycast(val2, val, ref val3, (distance.get_Value() != -1f) ? distance.get_Value() : float.PositiveInfinity, LayerMask.op_Implicit(layerMask)))
			{
				storeHitObject.set_Value(val3.get_collider().get_gameObject());
				storeHitPoint.set_Value(val3.get_point());
				storeHitNormal.set_Value(val3.get_normal());
				storeHitDistance.set_Value(val3.get_distance());
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			originGameObject = null;
			originPosition = Vector3.get_zero();
			direction = Vector3.get_zero();
			distance = -1f;
			layerMask = LayerMask.op_Implicit(-1);
			space = 1;
		}
	}
}
