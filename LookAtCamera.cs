using UnityEngine;

internal sealed class LookAtCamera : MonoBehaviour
{
	private Transform CameraT;

	private Transform T;

	public bool lockY;

	public bool flipAroundYAxis = true;

	public LookAtCamera()
		: this()
	{
	}

	private void Start()
	{
		float num = 0f;
		Camera[] allCameras = Camera.get_allCameras();
		foreach (Camera val in allCameras)
		{
			if (!val.get_orthographic() && (CameraT == null || val.get_depth() > num))
			{
				CameraT = val.get_transform();
				num = val.get_depth();
			}
		}
		T = this.get_transform();
	}

	private void Update()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		T.LookAt(CameraT);
		if (flipAroundYAxis)
		{
			T.set_rotation(Quaternion.Euler(0f, 180f, 0f) * T.get_rotation());
		}
		if (lockY)
		{
			Transform t = T;
			Quaternion rotation = T.get_rotation();
			Vector3 eulerAngles = rotation.get_eulerAngles();
			t.set_rotation(Quaternion.Euler(0f, eulerAngles.y, 0f));
		}
	}
}
