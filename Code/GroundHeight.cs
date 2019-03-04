using UnityEngine;

internal sealed class GroundHeight
{
	private float _groundHeight;

	public void SetGroundHeight(GroundHeightMarker groundHeightMarker)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = groundHeightMarker.get_transform().get_position();
		_groundHeight = position.y;
	}

	public float GetGroundHeight()
	{
		return _groundHeight;
	}
}
