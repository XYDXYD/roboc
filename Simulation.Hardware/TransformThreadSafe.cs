using UnityEngine;

namespace Simulation.Hardware
{
	internal struct TransformThreadSafe
	{
		public Vector3 forward;

		public Vector3 right;

		public Vector3 localPosition;

		public Vector3 position;

		public TransformThreadSafe(Vector3 forward, Vector3 right, Vector3 localPosition)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			this = default(TransformThreadSafe);
			this.forward = forward;
			this.right = right;
			this.localPosition = localPosition;
		}

		public TransformThreadSafe(Vector3 forward, Vector3 right, Vector3 localPosition, Vector3 position)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			this = new TransformThreadSafe(forward, right, localPosition);
			this.position = position;
		}
	}
}
