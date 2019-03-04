using System;
using UnityEngine;

[Serializable]
internal sealed class ConnectionPoint
{
	public Vector3 offset;

	public Vector3 direction;

	public ConnectionPoint(Vector3 o, Vector3 d)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		offset = o;
		direction = d;
	}
}
