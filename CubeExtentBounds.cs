using UnityEngine;

internal sealed class CubeExtentBounds
{
	public Int3 lowerBound = Int3.one * int.MaxValue;

	public Int3 upperBound = Int3.one * int.MinValue;

	public CubeExtentBounds()
	{
	}

	public CubeExtentBounds(CubeExtentBounds bounds)
	{
		lowerBound = bounds.lowerBound;
		upperBound = bounds.upperBound;
	}

	public void Rotate(Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		lowerBound = new Int3(rotation * lowerBound.ToVector3());
		upperBound = new Int3(rotation * upperBound.ToVector3());
	}
}
