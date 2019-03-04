using UnityEngine;

public class SkyDomeController : MonoBehaviour
{
	private bool cameraSet;

	private Quaternion _initialRotation;

	public SkyDomeController()
		: this()
	{
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!cameraSet)
		{
			this.get_transform().set_parent(Camera.get_main().get_transform());
			_initialRotation = this.get_transform().get_rotation();
			cameraSet = true;
		}
		this.get_transform().set_rotation(_initialRotation);
	}
}
