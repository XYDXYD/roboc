using UnityEngine;

internal static class PersistentCubeDataUtility
{
	private static float axisTolerance = 0.95f;

	public static bool IsFaceSelectable(this PersistentCubeData cube_data, CubeFace face)
	{
		switch (face)
		{
		case CubeFace.Back:
			return cube_data.selectableFaces.back;
		case CubeFace.Front:
			return cube_data.selectableFaces.front;
		case CubeFace.Up:
			return cube_data.selectableFaces.up;
		case CubeFace.Down:
			return cube_data.selectableFaces.down;
		case CubeFace.Left:
			return cube_data.selectableFaces.left;
		default:
			return cube_data.selectableFaces.right;
		}
	}

	public static bool IsFaceOccluding(this PersistentCubeData cube_data, CubeFace face)
	{
		switch (face)
		{
		case CubeFace.Back:
			return cube_data.occludingFaces.back;
		case CubeFace.Front:
			return cube_data.occludingFaces.front;
		case CubeFace.Up:
			return cube_data.occludingFaces.up;
		case CubeFace.Down:
			return cube_data.occludingFaces.down;
		case CubeFace.Left:
			return cube_data.occludingFaces.left;
		case CubeFace.Right:
			return cube_data.occludingFaces.right;
		default:
			return false;
		}
	}

	public static bool IsDirectionOccluding(this PersistentCubeData cube_data, Quaternion rotation, Vector3 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(rotation * Vector3.get_up(), direction);
		if (num > 0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Up);
		}
		if (num < -0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Down);
		}
		num = Vector3.Dot(rotation * Vector3.get_forward(), direction);
		if (num > 0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Front);
		}
		if (num < -0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Back);
		}
		num = Vector3.Dot(rotation * Vector3.get_right(), direction);
		if (num > 0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Right);
		}
		if (num < -0.95f)
		{
			return cube_data.IsFaceOccluding(CubeFace.Left);
		}
		return false;
	}

	public static bool IsDirectionSelectable(this PersistentCubeData cube_data, Quaternion rotation, Vector3 worldDirection, Vector3 localOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ConnectionPoint connection;
		return cube_data.IsDirectionSelectable(rotation, worldDirection, localOffset, out connection);
	}

	public static bool IsDirectionSelectable(this PersistentCubeData cube_data, Quaternion rotation, Vector3 worldDirection, Vector3 localOffset, out ConnectionPoint connection)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < cube_data.connectionPoints.Count; i++)
		{
			connection = cube_data.connectionPoints[i];
			if (connection.offset == localOffset)
			{
				float num = Vector3.Dot(rotation * connection.direction, worldDirection);
				if (num > 0.99f)
				{
					return true;
				}
			}
		}
		connection = null;
		return false;
	}

	public static bool IsPlacementFaceValid(this PersistentCubeData cube_data, Vector3 placement_normal)
	{
		if (placement_normal.x >= axisTolerance)
		{
			return cube_data.placementFaces.right;
		}
		if (placement_normal.x <= 0f - axisTolerance)
		{
			return cube_data.placementFaces.left;
		}
		if (placement_normal.y >= axisTolerance)
		{
			return cube_data.placementFaces.up;
		}
		if (placement_normal.y <= 0f - axisTolerance)
		{
			return cube_data.placementFaces.down;
		}
		if (placement_normal.z >= axisTolerance)
		{
			return cube_data.placementFaces.front;
		}
		if (placement_normal.z <= 0f - axisTolerance)
		{
			return cube_data.placementFaces.back;
		}
		return false;
	}
}
