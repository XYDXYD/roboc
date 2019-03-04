using UnityEngine;

namespace Simulation
{
	internal class ColliderData_Capsule : IColliderData
	{
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

		public ColliderData_Capsule(Collider c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			CapsuleCollider val = c as CapsuleCollider;
			Transform component = c.GetComponent<Transform>();
			Transform cubeRoot = GameUtility.GetCubeRoot(component);
			Vector3 lossyScale = component.get_lossyScale();
			float num = 2f * val.get_radius();
			Vector3 val2 = Vector3.get_zero();
			Vector3 zero = Vector3.get_zero();
			for (int i = 0; i < 3; i++)
			{
				if (i == val.get_direction())
				{
					val2.set_Item(i, Mathf.Max(val.get_height(), num) * 0.75f);
				}
				else
				{
					val2.set_Item(i, num * 0.95f);
				}
				int num2;
				val2.set_Item(num2 = i, val2.get_Item(num2) * lossyScale.get_Item(i));
				int num3 = i;
				Vector3 position = component.get_position();
				float num4 = position.get_Item(i);
				Vector3 position2 = cubeRoot.get_position();
				float num5 = num4 - position2.get_Item(i);
				Vector3 center = val.get_center();
				zero.set_Item(num3, num5 + center.get_Item(i) * lossyScale.get_Item(i));
			}
			val2 = component.get_rotation() * val2;
			for (int j = 0; j < 3; j++)
			{
				val2.set_Item(j, Mathf.Abs(val2.get_Item(j)));
			}
			size = val2;
			offset = zero;
		}
	}
}
