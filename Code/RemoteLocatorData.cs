using UnityEngine;

public struct RemoteLocatorData
{
	public Vector3 position;

	public int ownerId;

	public RemoteLocatorData(int ownerId_, Vector3 position_)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ownerId = ownerId_;
		position = position_;
	}

	public void SetValues(int ownerId_, Vector3 position_)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		position = position_;
		ownerId = ownerId_;
	}
}
