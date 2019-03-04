using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public Vector3 rotationVector;

	public RotateObject()
		: this()
	{
	}

	private void Update()
	{
		this.get_transform().Rotate(rotationVector.x * Time.get_deltaTime(), rotationVector.y * Time.get_deltaTime(), rotationVector.z * Time.get_deltaTime());
	}
}
