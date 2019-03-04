internal static class CubeFaceExtensions
{
	public static int NumberOfFaces()
	{
		return 6;
	}

	public static int NumberOfDirections()
	{
		return 7;
	}

	public static CubeFace Opposite(this CubeFace face)
	{
		switch (face)
		{
		case CubeFace.Up:
			return CubeFace.Down;
		case CubeFace.Down:
			return CubeFace.Up;
		case CubeFace.Front:
			return CubeFace.Back;
		case CubeFace.Back:
			return CubeFace.Front;
		case CubeFace.Right:
			return CubeFace.Left;
		case CubeFace.Left:
			return CubeFace.Right;
		default:
			return CubeFace.Other;
		}
	}
}
