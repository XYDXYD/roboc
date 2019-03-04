using UnityEngine;

internal sealed class WeaponRaycastInfo
{
	public Vector3 targetPoint = Vector3.get_zero();

	public Vector3 aimPoint = Vector3.get_zero();

	public bool hitSomething;

	public Rigidbody targetRigidbody;
}
