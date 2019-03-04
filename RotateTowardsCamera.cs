using UnityEngine;

internal class RotateTowardsCamera : MonoBehaviour
{
	public Vector3 localForward = Vector3.get_forward();

	private Transform _t;

	private Transform _mainCamera;

	public RotateTowardsCamera()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	private void Awake()
	{
		_t = this.get_transform();
	}

	private void OnEnable()
	{
		_mainCamera = Camera.get_main().get_transform();
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = _t.get_position();
		Vector3 val = position + _t.get_rotation() * localForward;
		Vector3 val2 = _mainCamera.get_position() - position;
		_t.LookAt(val, val2);
	}
}
