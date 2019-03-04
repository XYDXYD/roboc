internal static class MachineMassUtility
{
	public const int ACCUMULATION_MULTIPLIER = 1000;

	public const int DISPLAY_MULTIPLIER = 10;

	public static int GetMass(InstantiatedCube cube)
	{
		return (int)(cube.mass * cube.physcisMassScalar * 1000f);
	}

	public static float GetDisplayMass(int totalMass)
	{
		return (float)(totalMass * 10) / 1000f;
	}
}
