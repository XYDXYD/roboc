using UnityEngine;

internal sealed class PlayerMachineMotionHistoryFrame : PlayerHistoryFrame
{
	internal sealed class RigidBodyState
	{
		public Vector3 position;

		public Vector3 worldCOM;

		public Quaternion rotation;

		public Vector3 centreOfMass;

		public Vector3 angularVelocity;

		public RigidBodyState()
		{
		}

		public RigidBodyState(Rigidbody rb)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			position = rb.get_position();
			rotation = rb.get_rotation();
			centreOfMass = rb.get_centerOfMass();
			worldCOM = rb.get_worldCenterOfMass();
			angularVelocity = rb.get_angularVelocity();
		}
	}

	public RigidBodyState rbState;

	public WeaponRaycastInfo weaponRaycast;

	public PlayerMachineMotionHistoryFrame()
	{
	}

	public PlayerMachineMotionHistoryFrame(RigidBodyState _rbState, WeaponRaycastInfo _weaponRaycast)
	{
		rbState = _rbState;
		weaponRaycast = _weaponRaycast;
	}

	public PlayerMachineMotionHistoryFrame(Rigidbody rbData, WeaponRaycastInfo _weaponRaycast)
	{
		rbState = new RigidBodyState(rbData);
		weaponRaycast = _weaponRaycast;
	}
}
