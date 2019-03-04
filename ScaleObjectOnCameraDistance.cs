using UnityEngine;

internal sealed class ScaleObjectOnCameraDistance : MonoBehaviour
{
	public float closestDistance = 1f;

	public float closestScale = 1f;

	public float farthestDistance = 5f;

	public float farthestScale = 2f;

	private float distanceDiff;

	private float scaleDiff;

	private Transform T;

	private Transform mainCameraT;

	private Vector3 _initialLocalScale;

	public ScaleObjectOnCameraDistance()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		T = this.get_transform();
		mainCameraT = Camera.get_main().get_transform();
		_initialLocalScale = T.get_localScale();
		distanceDiff = Mathf.Clamp(farthestDistance - closestDistance, 0.001f, farthestDistance);
		scaleDiff = Mathf.Clamp(farthestScale - closestScale, 0.001f, farthestScale);
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(T.get_position(), mainCameraT.get_position());
		float num2 = Mathf.Clamp01((num - closestDistance) / distanceDiff);
		float num3 = closestScale + scaleDiff * num2;
		T.set_localScale(_initialLocalScale * num3);
	}
}
