using UnityEngine;

[AddComponentMenu("Network/Utilities/SpectatorCamera")]
internal sealed class SpectatorCamera : MonoBehaviour
{
	public float rotateSpeed = 300f;

	public float moveSpeed = 100f;

	private Vector3 positionDelta = Vector3.get_zero();

	private Quaternion rotationDeltaHorizontal = Quaternion.get_identity();

	private Quaternion rotationDeltaVertical = Quaternion.get_identity();

	public SpectatorCamera()
		: this()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0032: Unknown result type (might be due to invalid IL or missing references)


	public void Start()
	{
	}

	private void SetAsCurrent()
	{
		Camera[] array = Object.FindObjectsOfType(typeof(Camera)) as Camera[];
		Camera component = this.GetComponent<Camera>();
		Camera[] array2 = array;
		foreach (Camera val in array2)
		{
			if (val != component)
			{
				val.set_enabled(false);
			}
		}
	}

	private void Update()
	{
		UpdateInput();
		UpdatePosition();
	}

	private void UpdateInput()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyUp(313))
		{
			Camera component = this.GetComponent<Camera>();
			component.set_depth(component.get_depth() * -1f);
		}
		float axis = Input.GetAxis("Mouse X");
		float axis2 = Input.GetAxis("Mouse Y");
		rotationDeltaHorizontal = Quaternion.AngleAxis(axis * rotateSpeed * Time.get_deltaTime(), Vector3.get_up());
		rotationDeltaVertical = Quaternion.AngleAxis((0f - axis2) * rotateSpeed * Time.get_deltaTime(), Vector3.get_right());
		if (Input.GetKey(119))
		{
			positionDelta += this.get_transform().get_forward();
		}
		if (Input.GetKey(115))
		{
			positionDelta -= this.get_transform().get_forward();
		}
		if (Input.GetKey(100))
		{
			positionDelta += this.get_transform().get_right();
		}
		if (Input.GetKey(97))
		{
			positionDelta -= this.get_transform().get_right();
		}
		if (Input.GetKey(32))
		{
			positionDelta += this.get_transform().get_up();
		}
		if (Input.GetKey(304))
		{
			positionDelta -= this.get_transform().get_up();
		}
		positionDelta *= moveSpeed * Time.get_deltaTime();
	}

	private void UpdatePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = this.get_transform();
		transform.set_position(transform.get_position() + positionDelta);
		Transform transform2 = this.get_transform();
		transform2.set_rotation(transform2.get_rotation() * rotationDeltaHorizontal);
		Transform transform3 = this.get_transform();
		transform3.set_rotation(transform3.get_rotation() * rotationDeltaVertical);
		Quaternion rotation = this.get_transform().get_rotation();
		Vector3 eulerAngles = rotation.get_eulerAngles();
		eulerAngles.z = 0f;
		rotation.set_eulerAngles(eulerAngles);
		this.get_transform().set_rotation(rotation);
		positionDelta = Vector3.get_zero();
		rotationDeltaHorizontal = Quaternion.get_identity();
		rotationDeltaVertical = Quaternion.get_identity();
	}
}
