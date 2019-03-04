using UnityEngine;

internal sealed class SuspensionLookAt : MonoBehaviour
{
	public Transform LookAtObject;

	private Transform T;

	private Transform _parent;

	private Transform _targetObject;

	public SuspensionLookAt()
		: this()
	{
	}

	public void InitialiseLook(Transform lookAt, Transform parent, bool inverseDirection)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		T = this.get_transform();
		LookAtObject = lookAt;
		_parent = parent;
		Vector3 val = LookAtObject.get_position() - T.get_position();
		float num = Vector3.Angle(T.get_forward(), val.get_normalized());
		if (inverseDirection)
		{
			num *= -1f;
		}
		Vector3 val2 = LookAtObject.get_position() - T.get_position();
		Vector3 val3 = Vector3.Cross(val2.get_normalized(), _parent.get_up());
		Quaternion val4 = Quaternion.AngleAxis(num, val3);
		_targetObject = new GameObject("SuspensionTargetObject").get_transform();
		_targetObject.set_parent(LookAtObject);
		_targetObject.set_position(val4 * val2 + T.get_position());
	}

	public void UpdateRotation()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (_targetObject != null)
		{
			T.LookAt(_targetObject.get_position(), _parent.get_up());
		}
	}
}
