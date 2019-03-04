using UnityEngine;

internal sealed class SuspensionStretch : MonoBehaviour
{
	public Transform TargetStretch;

	private Transform T;

	private float _initialDistance = 1f;

	public SuspensionStretch()
		: this()
	{
	}

	public void InitialiseStretch(Transform target)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		T = this.get_transform();
		TargetStretch = target;
		if (TargetStretch != null)
		{
			_initialDistance = Vector3.Distance(TargetStretch.get_position(), T.get_position());
			if (_initialDistance == 0f)
			{
				_initialDistance = 1f;
			}
		}
	}

	private void Update()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (TargetStretch != null)
		{
			float num = Vector3.Distance(TargetStretch.get_position(), T.get_position());
			Vector3 localScale = T.get_localScale();
			localScale.z = num / _initialDistance;
			T.set_localScale(localScale);
		}
	}
}
