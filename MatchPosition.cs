using UnityEngine;

internal class MatchPosition : MonoBehaviour
{
	public Transform target;

	private Transform _transform;

	public MatchPosition()
		: this()
	{
	}

	private void Awake()
	{
		_transform = this.get_transform();
	}

	private void LateUpdate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_transform.set_position(target.get_position());
	}
}
