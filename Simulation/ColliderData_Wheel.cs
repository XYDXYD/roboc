using UnityEngine;

namespace Simulation
{
	internal class ColliderData_Wheel : IColliderData
	{
		private const float WHEEL_WIDTH = 0.01f;

		private const float CYLINDER_LENGTH_SCALE = 0.75f;

		private const float CYLINDER_RADIUS_SCALE = 0.95f;

		public Vector3 size
		{
			get;
			private set;
		}

		public Vector3 offset
		{
			get;
			private set;
		}

		public ColliderData_Wheel(Collider c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			WheelCollider val = c as WheelCollider;
			Transform component = c.GetComponent<Transform>();
			Transform cubeRoot = GameUtility.GetCubeRoot(component);
			Vector3 lossyScale = component.get_lossyScale();
			Vector3 val2 = Vector3.get_zero();
			val2.x = 0.01f * lossyScale.x * 0.75f;
			val2.y = 2f * val.get_radius() * lossyScale.y * 0.95f;
			val2.z = 2f * val.get_radius() * lossyScale.z * 0.95f;
			val2 = component.get_rotation() * val2;
			for (int i = 0; i < 3; i++)
			{
				val2.set_Item(i, Mathf.Abs(val2.get_Item(i)));
			}
			size = val2;
			offset = component.get_position() - cubeRoot.get_position() + component.get_rotation() * val.get_center();
		}
	}
}
