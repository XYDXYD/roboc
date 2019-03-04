using UnityEngine;

internal sealed class MockRigidbody : MonoBehaviour
{
	public Transform T;

	private Vector3 _velocity;

	private Vector3 _angularVelocity;

	private Vector3 _g;

	public MockRigidbody()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_g = Physics.get_gravity();
		T = this.get_transform();
	}

	private void FixedUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_velocity += _g * Time.get_fixedDeltaTime();
		Transform t = T;
		t.set_position(t.get_position() + _velocity * Time.get_fixedDeltaTime());
		T.Rotate(_angularVelocity * Time.get_fixedDeltaTime());
	}

	public void AddForce(Vector3 force, ForceMode forceMode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((int)forceMode == 2)
		{
			_velocity += force;
		}
	}

	public void AddTorque(Vector3 rot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_angularVelocity += rot;
	}
}
