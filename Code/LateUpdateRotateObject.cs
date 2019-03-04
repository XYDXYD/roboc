using UnityEngine;

public class LateUpdateRotateObject : MonoBehaviour
{
	public Vector3 rotationVector;

	public float speed;

	private Transform _transform;

	public LateUpdateRotateObject()
		: this()
	{
	}

	private void Awake()
	{
		_transform = this.get_transform();
	}

	private void LateUpdate()
	{
		float num = Time.get_deltaTime() * speed;
		_transform.Rotate(rotationVector.x * num, rotationVector.y * num, rotationVector.z * num);
	}
}
