using UnityEngine;

internal sealed class ContinuousRotation : MonoBehaviour
{
	public Vector3 Axis = Vector3.get_up();

	public float DegreesPerSecondRotation = 30f;

	private Transform T;

	public ContinuousRotation()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	private void Awake()
	{
		T = this.get_transform();
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		T.Rotate(Axis, DegreesPerSecondRotation * Time.get_deltaTime(), 0);
	}
}
