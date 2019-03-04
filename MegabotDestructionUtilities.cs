internal static class MegabotDestructionUtilities
{
	internal static HitCubeInfo GetCubeHitInfo(InstantiatedCube cube, int newHealth)
	{
		HitCubeInfo result = default(HitCubeInfo);
		result.gridLoc = cube.gridPos;
		if (!cube.isDestroyed)
		{
			result.damage = cube.health - newHealth;
			result.destroyed = (newHealth == 0);
		}
		else
		{
			result.damage = 0;
			result.destroyed = false;
		}
		return result;
	}
}
