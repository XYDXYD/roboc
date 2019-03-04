using UnityEngine;

internal class ScalableProjector : MonoBehaviour
{
	private Transform _transform;

	private Projector _projector;

	private float _initialSize;

	public ScalableProjector()
		: this()
	{
	}

	private void Start()
	{
		_transform = this.get_transform();
		_projector = this.GetComponent<Projector>();
		_initialSize = _projector.get_orthographicSize();
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Projector projector = _projector;
		float initialSize = _initialSize;
		Vector3 lossyScale = _transform.get_lossyScale();
		projector.set_orthographicSize(initialSize * lossyScale.x);
	}
}
