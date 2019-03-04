using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPhysics2D
{
	[TaskCategory("Basic/Physics2D")]
	[TaskDescription("Casts a circle against all colliders in the scene. Returns success if a collider was hit.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=118")]
	public class Circlecast : Action
	{
		[Tooltip("Starts the circlecast at the GameObject's position. If null the originPosition will be used.")]
		public SharedGameObject originGameObject;

		[Tooltip("Starts the circlecast at the position. Only used if originGameObject is null.")]
		public SharedVector2 originPosition;

		[Tooltip("The radius of the circlecast")]
		public SharedFloat radius;

		[Tooltip("The direction of the circlecast")]
		public SharedVector2 direction;

		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public SharedFloat distance = -1f;

		[Tooltip("Selectively ignore colliders.")]
		public LayerMask layerMask = LayerMask.op_Implicit(-1);

		[Tooltip("Use world or local space. The direction is in world space if no GameObject is specified.")]
		public Space space = 1;

		[SharedRequired]
		[Tooltip("Stores the hit object of the circlecast.")]
		public SharedGameObject storeHitObject;

		[SharedRequired]
		[Tooltip("Stores the hit point of the circlecast.")]
		public SharedVector2 storeHitPoint;

		[SharedRequired]
		[Tooltip("Stores the hit normal of the circlecast.")]
		public SharedVector2 storeHitNormal;

		[SharedRequired]
		[Tooltip("Stores the hit distance of the circlecast.")]
		public SharedFloat storeHitDistance;

		public Circlecast()
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
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = direction.get_Value();
			Vector2 val2;
			if (originGameObject.get_Value() != null)
			{
				val2 = Vector2.op_Implicit(originGameObject.get_Value().get_transform().get_position());
				if ((int)space == 1)
				{
					val = Vector2.op_Implicit(originGameObject.get_Value().get_transform().TransformDirection(Vector2.op_Implicit(direction.get_Value())));
				}
			}
			else
			{
				val2 = originPosition.get_Value();
			}
			RaycastHit2D val3 = Physics2D.CircleCast(val2, radius.get_Value(), val, (distance.get_Value() != -1f) ? distance.get_Value() : float.PositiveInfinity, LayerMask.op_Implicit(layerMask));
			if (val3.get_collider() != null)
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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			originGameObject = null;
			originPosition = Vector2.get_zero();
			direction = Vector2.get_zero();
			radius = 0f;
			distance = -1f;
			layerMask = LayerMask.op_Implicit(-1);
			space = 1;
		}
	}
}
