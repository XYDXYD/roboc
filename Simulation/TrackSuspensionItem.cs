using System;
using UnityEngine;

namespace Simulation
{
	[Serializable]
	internal sealed class TrackSuspensionItem
	{
		public Transform raycastingObj;

		public Transform transform;

		public Transform lookAtObj;

		public float raycastDist = 1f;

		[NonSerialized]
		public Vector3 colliderOffset = Vector3.get_zero();

		[NonSerialized]
		public SuspensionLookAt lookAt;

		public void InitSuspension(Transform root, Transform baseRotation)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			colliderOffset = Quaternion.Inverse(root.get_rotation()) * (transform.get_position() - raycastingObj.get_position());
			if (lookAtObj != null)
			{
				lookAt = lookAtObj.get_gameObject().AddComponent<SuspensionLookAt>();
				lookAt.InitialiseLook(transform, baseRotation, inverseDirection: false);
			}
		}
	}
}
