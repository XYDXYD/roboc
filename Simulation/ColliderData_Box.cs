using UnityEngine;

namespace Simulation
{
	internal class ColliderData_Box : IColliderData
	{
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

		public ColliderData_Box(ref Bounds bounds, ref Vector3 position)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			size = bounds.get_size();
			offset = bounds.get_center() - position;
		}

		public ColliderData_Box(Collider c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			BoxCollider val = c as BoxCollider;
			Transform component = c.GetComponent<Transform>();
			Transform cubeRoot = GameUtility.GetCubeRoot(component);
			Vector3 lossyScale = component.get_lossyScale();
			Vector3 val2 = Vector3.get_zero();
			for (int i = 0; i < 3; i++)
			{
				int num = i;
				Vector3 size = val.get_size();
				val2.set_Item(num, size.get_Item(i) * lossyScale.get_Item(i));
			}
			val2 = component.get_rotation() * val2;
			for (int j = 0; j < 3; j++)
			{
				val2.set_Item(j, Mathf.Abs(val2.get_Item(j)));
			}
			this.size = val2;
			offset = component.get_position() - cubeRoot.get_position() + component.get_rotation() * val.get_center();
		}
	}
}
