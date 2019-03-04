using UnityEngine;

namespace Simulation
{
	internal class ColliderData_Generic : IColliderData
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

		public ColliderData_Generic(Collider c)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Transform component = c.GetComponent<Transform>();
			Transform cubeRoot = GameUtility.GetCubeRoot(component);
			Vector3 size = Vector3.get_zero();
			Quaternion rotation = component.get_rotation();
			Bounds bounds = c.get_bounds();
			size = rotation * bounds.get_size();
			for (int i = 0; i < 3; i++)
			{
				size.set_Item(i, Mathf.Abs(size.get_Item(i)));
			}
			this.size = size;
			offset = component.get_position() - cubeRoot.get_position();
		}
	}
}
