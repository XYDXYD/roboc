using UnityEngine;

[ExecuteInEditMode]
internal sealed class RotateTransform : MonoBehaviour
{
	public float Rotation;

	public Vector3 Axis = Vector3.get_forward();

	public Transform[] TargetTransforms;

	public RotateTransform()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	private void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Transform[] targetTransforms = TargetTransforms;
		foreach (Transform val in targetTransforms)
		{
			val.set_localRotation(Quaternion.AngleAxis(Rotation, Axis));
		}
	}
}
