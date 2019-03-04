using Svelto.IoC;

internal sealed class CubeSelectVisibilityCheckerNormal : ICubeSelectVisibilityChecker
{
	[Inject]
	internal ICubeList cubeList
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	public bool ShouldCubeBeVisibleInDepot(CubeTypeID type, CubeCategory cubeCategory)
	{
		if (!cubeInventory.IsCubeOwned(type))
		{
			return false;
		}
		if (cubeList.CubeTypeDataOf(type).active && cubeList.CubeTypeDataOf(type).cubeData.category == cubeCategory && (cubeList.GetBuildModeVisibility(type) == BuildVisibility.Mothership || cubeList.GetBuildModeVisibility(type) == BuildVisibility.All))
		{
			return true;
		}
		return false;
	}
}
