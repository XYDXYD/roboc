using UnityEngine;

internal sealed class RaycastUtility
{
	private static Collider[] _emptyColliders = (Collider[])new Collider[0];

	private static Collider[] _overlapColliders = (Collider[])new Collider[200];

	private const int MAX_INTERSECTIONS = 200;

	public static int OverlapSphere(ref Vector3 position, float radius, int layerMask, out Collider[] hitColliders)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int num = Physics.OverlapSphereNonAlloc(position, radius, _overlapColliders, layerMask);
		if (num > 0)
		{
			hitColliders = _overlapColliders;
			return num;
		}
		hitColliders = _emptyColliders;
		return 0;
	}
}
