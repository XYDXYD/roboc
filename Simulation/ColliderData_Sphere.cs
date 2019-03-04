using UnityEngine;

namespace Simulation
{
	internal class ColliderData_Sphere : IColliderData
	{
		private const float SIZE_SCALE = 0.6f;

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

		public ColliderData_Sphere(Collider c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			SphereCollider val = c as SphereCollider;
			Transform component = c.GetComponent<Transform>();
			Transform cubeRoot = GameUtility.GetCubeRoot(component);
			Vector3 lossyScale = component.get_lossyScale();
			float num = 2f * val.get_radius();
			Vector3 val2 = Vector3.get_zero();
			for (int i = 0; i < 3; i++)
			{
				val2.set_Item(i, num * lossyScale.get_Item(i));
			}
			val2 = component.get_rotation() * val2;
			for (int j = 0; j < 3; j++)
			{
				val2.set_Item(j, Mathf.Abs(val2.get_Item(j)));
			}
			size = val2 * 0.6f;
			offset = component.get_position() - cubeRoot.get_position();
		}
	}
}
