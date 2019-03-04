using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class BuildShieldParametersData
	{
		public Vector3 hitPoint;

		public Rigidbody rigidBody;

		public Vector3 hitNormal;

		public Vector3 position;

		public Quaternion rotation;

		public int owner;

		public bool isMine;

		public bool isOnMyTeam;

		public void SetValues(Vector3 hitPoint_, Vector3 hitNormal_, Rigidbody rigidBody_, Vector3 position_, Quaternion rotation_, int owner_, bool isMine_, bool isOnMyTeam_)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			hitPoint = hitPoint_;
			rigidBody = rigidBody_;
			hitNormal = hitNormal_;
			position = position_;
			rotation = rotation_;
			owner = owner_;
			isMine = isMine_;
			isOnMyTeam = isOnMyTeam_;
		}
	}
}
